using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Service.Interfaces
{
    /// <summary>
    /// Предоставляет методы для работы с кэшем.
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Получает значение из кэша по указанному ключу или устанавливает его с помощью фабрики, если кэш пуст.
        /// </summary>
        /// <typeparam name="T">Тип кэшируемых данных.</typeparam>
        /// <param name="key">Ключ кэша.</param>
        /// <param name="factory">Фабрика для создания данных при отсутствии в кэше.</param>
        /// <param name="expiration">Время жизни кэшированных данных. Если не указано — используется значение по умолчанию.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Кэшированные или созданные данные.</returns>
        Task<T> GetOrSetAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет данные из кэша по указанному ключу.
        /// </summary>
        /// <param name="key">Ключ кэша.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удаляет все данные из кэша, соответствующие заданному шаблону.
        /// </summary>
        /// <param name="pattern">Шаблон ключей (например, "subject:*").</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    }
}