using System;
using System.Collections.Generic;
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
        public SubjectRepository(AppDbContext context, ICachingService cache) : base(context)
        {
            ArgumentNullException.ThrowIfNull(cache, nameof(cache));
            _subjects = context.Set<Subject>();
            _cache = cache;
        }

        /// <summary>
        /// Проверяет, существует ли предмет с указанным названием.
        /// </summary>
        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            
            return await _subjects
                .AsNoTracking()
                .AnyAsync(e => e.Title == name && (excludeId == null || e.Id != excludeId), ct)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<List<Subject>> GetAllAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrSetAsync(
                "subject:all",
                // ПРЯМОЙ ВЫЗОВ: Берем данные без отслеживания, чтобы не захламлять кэш и логи
                () => _subjects.AsNoTracking().ToListAsync(ct),
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