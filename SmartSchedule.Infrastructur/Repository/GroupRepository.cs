
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Infrastructure.Data;
using SmartSchedule.Core.Service.Interfaces; // Для ICachingService

namespace SmartSchedule.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с Группами.
    /// Реализует кэширование списка групп.
    /// </summary>
    public class GroupRepository : BaseRepository<Group, int, AppDbContext>, IGroupRepository
    {
        private readonly DbSet<Group> _group;
        private readonly ICachingService _cache;
        private const string CacheKey = "groups:all";

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GroupRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        /// <param name="cache">Сервис кэширования.</param>
        public GroupRepository(AppDbContext context, ICachingService cache) : base(context)
        {
            _group = context.Groups;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Проверяет уникальность названия группы.
        /// Не использует кэш, чтобы гарантировать точность данных.
        /// </summary>
        /// <param name="name">Название группы.</param>
        /// <param name="excludeId">ID группы для исключения из проверки (при обновлении).</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>True, если группа с таким именем существует.</returns>
        public async Task<bool> ExistsByAsync(string name, int? excludeId = null, CancellationToken ct = default)
        {
            return await _group
                .AnyAsync(e => e.Name == name && (excludeId == null || e.Id != excludeId), ct)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<List<Group>> GetAllAsync(CancellationToken ct = default)
        {
            return await _cache.GetOrSetAsync(
                CacheKey,
                () => base.GetAllAsync(ct),
                TimeSpan.FromHours(4), 
                ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<Group> CreateAsync(Group entity, CancellationToken ct = default)
        {
            var result = await base.CreateAsync(entity, ct).ConfigureAwait(false);
            await _cache.RemoveAsync(CacheKey, ct).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public override async Task UpdateAsync(Group entity, CancellationToken ct = default)
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
