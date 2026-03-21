using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Infrastructure.Data;

namespace SmartSchedule.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с Временными слотами.
    /// Данные кэшируются на длительный срок.
    /// </summary>
    public class TimeSlotRepository : BaseRepository<TimeSlot, int, AppDbContext>, ITimeSlotRepository
    {
        private readonly ICachingService _cache;
        private const string CacheKey = "timeslots:all";

        /// <summary>
        /// Конструктор репозитория TimeSlot.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="cache">Сервис кэширования.</param>
        public TimeSlotRepository(AppDbContext context, ICachingService cache) : base(context)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <inheritdoc />
        public override async Task<List<TimeSlot>> GetAllAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrSetAsync(
                CacheKey,
                () => base.GetAllAsync(ct),
                TimeSpan.FromHours(24), // Слоты меняются раз в год
                ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<TimeSlot> CreateAsync(TimeSlot entity, CancellationToken ct = default)
        {
            var result = await base.CreateAsync(entity, ct).ConfigureAwait(false);
            await _cache.RemoveAsync(CacheKey, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(TimeSlot entity, CancellationToken ct = default)
        {
            await base.UpdateAsync(entity, ct).ConfigureAwait(false);
            await _cache.RemoveAsync(CacheKey, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            await base.DeleteByIdAsync(id, ct).ConfigureAwait(false);
            await _cache.RemoveAsync(CacheKey, ct).ConfigureAwait(false);
        }
    }
}