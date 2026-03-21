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
/// Репозиторий для работы с должностями преподавателей.
/// Данные кэшируются в Redis с автоматической инвалидацией при изменении.
/// </summary>
public class PositionRepository : BaseRepository<Position, int, AppDbContext>, IPositionRepository
{
    private readonly DbSet<Position> _positions;
    private readonly ICachingService _cache;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="PositionRepository"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="cache">Сервис кэширования.</param>
    /// <exception cref="ArgumentNullException">
    /// Выбрасывается, если <paramref name="context"/> или <paramref name="cache"/> равны <see langword="null"/>.
    /// </exception>
    public PositionRepository(AppDbContext context, ICachingService cache) : base(context)
    {
        ArgumentNullException.ThrowIfNull(cache, nameof(cache));
        _positions = context.Set<Position>();
        _cache = cache;
    }

    /// <summary>
    /// Проверяет, существует ли должность с указанным названием.
    /// </summary>
    /// <param name="name">Название должности.</param>
    /// <param name="excludeId">Идентификатор должности, которую следует исключить из проверки.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns><see langword="true"/>, если должность с таким названием уже существует; иначе — <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="name"/> равен <see langword="null"/> или пуст.</exception>
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        return await _positions
            .AnyAsync(e => e.Name == name && (excludeId == null || e.Id != excludeId), ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<List<Position>> GetAllAsync(CancellationToken ct = default)
    {
        return await _cache.GetOrSetAsync(
            "positions:all",
            () => base.GetAllAsync(ct),
            TimeSpan.FromHours(24),
            ct).ConfigureAwait(false);
    }
    
    /// <inheritdoc />
    public override async Task<Position> CreateAsync(Position entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        var result = await base.CreateAsync(entity, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("positions:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false); 
        return result;
    }

    /// <inheritdoc />
    public override async Task UpdateAsync(Position entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        await base.UpdateAsync(entity, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("positions:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false); 
    }

    /// <inheritdoc />
    public override async Task DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        await base.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("positions:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false); 
    }
}