using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SmartSchedule.Core.Service.Interfaces;

namespace SmartSchedule.Infrastructure.Caching
{
    /// <summary>
    /// Сервис кэширования через Redis.
    /// Реализует паттерн Cache-Aside с защитой от Cache Stampede (через Keyed Locking).
    /// При недоступности Redis продолжает работу, обращаясь напрямую к БД.
    /// </summary>
    public class RedisCachingService : ICachingService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _connection;

        // Коллекция семафоров для блокировки запросов по ключам
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RedisCachingService"/>.
        /// </summary>
        /// <param name="redis">Мультиплексор подключения к Redis.</param>
        public RedisCachingService(IConnectionMultiplexer redis)
        {
            ArgumentNullException.ThrowIfNull(redis, nameof(redis));
            _connection = redis;
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// Получает данные из кэша или выполняет фабричный метод для их загрузки (с защитой от гонки потоков).
        /// </summary>
        /// <typeparam name="T">Тип данных.</typeparam>
        /// <param name="key">Ключ кэша.</param>
        /// <param name="factory">Метод загрузки данных из источника (БД), если кэш пуст.</param>
        /// <param name="expiration">Время жизни кэша (по умолчанию 30 минут).</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Данные типа <typeparamref name="T"/>.</returns>
        public async Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));
            ArgumentNullException.ThrowIfNull(factory, nameof(factory));

           
            if (_connection.IsConnected)
            {
                try
                {
                    var val = await _db.StringGetAsync(key).ConfigureAwait(false);
                    if (!val.IsNull)
                    {
                        var res = Deserialize<T>(val);
                        if (res != null) return res;
                    }
                }
                catch (RedisException)
                {
                    
                }
            }

            
            var myLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            
            var entered = await myLock.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);

            try
            {
                // Если зашли в семафор, делаем повторную проверку (Double Check Locking)
                if (entered && _connection.IsConnected)
                {
                    try
                    {
                        var val = await _db.StringGetAsync(key).ConfigureAwait(false);
                        if (!val.IsNull)
                        {
                            var res = Deserialize<T>(val);
                            if (res != null) return res;
                        }
                    }
                    catch (RedisException)
                    {
                        
                    }
                }

                // 4. Выполняем запрос к БД (Factory)
                var result = await factory().ConfigureAwait(false);

                // 5. Сохраняем результат в Redis
                if (result != null && _connection.IsConnected)
                {
                    try
                    {
                        var json = JsonSerializer.Serialize(result, _jsonOptions);
                        await _db.StringSetAsync(
                            key,
                            json,
                            expiration ?? TimeSpan.FromMinutes(30)
                        ).ConfigureAwait(false);
                    }
                    catch (RedisException) {  }
                    catch (JsonException) {}
                }

                return result;
            }
            finally
            {
                if (entered)
                {
                    myLock.Release();
                }
            }
        }

        /// <summary>
        /// Удаляет запись из кэша по ключу.
        /// </summary>
        /// <param name="key">Ключ записи.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));

            if (!_connection.IsConnected) return;

            try
            {
                await _db.KeyDeleteAsync(key).ConfigureAwait(false);
            }
            catch (RedisException)
            {
                // Ловим ошибку Redis
            }
        }

        /// <summary>
        /// Удаляет группу записей по шаблону ключа.
        /// </summary>
        /// <param name="pattern">Шаблон ключа (например "users:*").</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(pattern, nameof(pattern));

            if (!_connection.IsConnected) return;

            try
            {
                var server = _connection.GetServer(_connection.GetEndPoints()[0]);

                // Итерация по ключам тоже должна быть сконфигурирована
                await foreach (var key in server.KeysAsync(pattern: pattern).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await _db.KeyDeleteAsync(key).ConfigureAwait(false);
                }
            }
            catch (RedisException)
            {
                // Ловим ошибку Redis
            }
            catch (NotSupportedException)
            {
                // Игнорируем, если сервер не поддерживает команду
            }
        }

        /// <summary>
        /// Безопасная десериализация объекта из JSON.
        /// </summary>
        private static T? Deserialize<T>(RedisValue value)
        {
            if (value.IsNullOrEmpty) return default;

            try
            {
                return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}