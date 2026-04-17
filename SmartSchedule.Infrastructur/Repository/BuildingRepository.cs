using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Infrastructure.Data;

namespace SmartSchedule.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы со зданиями (корпусами).
/// Все данные кэшируются в Redis с автоматической инвалидацией при изменении.
/// </summary>
public class BuildingRepository : BaseRepository<Building, int, AppDbContext>, IBuildingRepository
{
    private readonly DbSet<Building> _buildings;
    private readonly ICachingService _cache;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="BuildingRepository"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="cache">Сервис кэширования.</param>
    /// <exception cref="ArgumentNullException">
    /// Выбрасывается, если <paramref name="context"/> или <paramref name="cache"/> равны <see langword="null"/>.
    /// </exception>
    public BuildingRepository(AppDbContext context, ICachingService cache) : base(context)
    {
        ArgumentNullException.ThrowIfNull(cache, nameof(cache));
        _buildings = context.Set<Building>();
        _cache = cache;
    }

    /// <summary>
    /// Проверяет, существует ли здание с указанным названием.
    /// </summary>
    /// <param name="name">Название здания.</param>
    /// <param name="excludeId">Идентификатор здания, которое следует исключить из проверки.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns><see langword="true"/>, если здание с таким названием уже существует; иначе — <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="name"/> равен <see langword="null"/> или пуст.</exception>
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        return await _buildings
            .AsNoTracking()
            .AnyAsync(e => e.Name == name && (excludeId == null || e.Id != excludeId), ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<List<Building>> GetAllAsync(CancellationToken ct = default)
    {
        return await _cache.GetOrSetAsync(
            "buildings:all",
            () => base.GetAllAsync(ct),
            TimeSpan.FromHours(24),
            ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<Building> CreateAsync(Building entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        var result = await base.CreateAsync(entity, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("buildings:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:all", ct).ConfigureAwait(false); 
        return result;
    }

    /// <inheritdoc />
    public override async Task UpdateAsync(Building entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        await base.UpdateAsync(entity, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("buildings:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:all", ct).ConfigureAwait(false); 
    }

    /// <inheritdoc />
    public override async Task DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        await base.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("buildings:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:all", ct).ConfigureAwait(false); 
    }
}