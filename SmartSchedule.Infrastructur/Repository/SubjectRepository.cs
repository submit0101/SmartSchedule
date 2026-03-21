using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Infrastructure.Data;

namespace SmartSchedule.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с сущностями предметов.
    /// Реализует кэширование и валидацию уникальности названий.
    /// </summary>
    public class SubjectRepository : BaseRepository<Subject, int, AppDbContext>, ISubjectRepository
    {
        private readonly DbSet<Subject> _subjects;
        private readonly ICachingService _cache;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SubjectRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="cache">Сервис кэширования.</param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если <paramref name="context"/> или <paramref name="cache"/> равны <see langword="null"/>.
        /// </exception>
        public SubjectRepository(AppDbContext context, ICachingService cache) : base(context)
        {
            ArgumentNullException.ThrowIfNull(cache, nameof(cache));
            _subjects = context.Set<Subject>();
            _cache = cache;
        }

        /// <summary>
        /// Проверяет, существует ли предмет с указанным названием.
        /// </summary>
        /// <param name="name">Название предмета.</param>
        /// <param name="excludeId">Идентификатор предмета, который следует исключить из проверки.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns><see langword="true"/>, если предмет с таким названием уже существует; иначе — <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="name"/> равен <see langword="null"/> или пуст.</exception>
        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            return await _subjects
                .AnyAsync(e => e.Title == name && (excludeId == null || e.Id != excludeId), ct)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<List<Subject>> GetAllAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrSetAsync(
                "subject:all",
                () => base.GetAllAsync(ct),
                TimeSpan.FromMinutes(5),
                ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<Subject> CreateAsync(Subject entity, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            var result = await base.CreateAsync(entity, ct).ConfigureAwait(false);
            await _cache.RemoveAsync("subject:all", ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(Subject entity, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            await base.UpdateAsync(entity, ct).ConfigureAwait(false);
            await _cache.RemoveAsync("subject:all", ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            await base.DeleteByIdAsync(id, ct).ConfigureAwait(false);
            await _cache.RemoveAsync("subject:all", ct).ConfigureAwait(false);
        }
    }
}