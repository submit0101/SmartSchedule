using Microsoft.EntityFrameworkCore;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Core.Entities.Abstractions;
using SmartSchedule.Core.Repositories;
using System.Linq;
using System.Linq.Expressions;

namespace SmartSchedule.Infrastructure.Repositories;

/// <summary>
/// Базовый репозиторий.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
/// <typeparam name="TId">Тип идентификатора сущности.</typeparam>
/// <typeparam name="TDbContext">Тип контекста базы данных.</typeparam>
public abstract class BaseRepository<TEntity, TId, TDbContext> : IBaseRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
    where TDbContext : DbContext
{
    #region Поля

    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly TDbContext _context;

    /// <summary>
    /// Множество (сет) сущностей.
    /// </summary>
    private readonly DbSet<TEntity> _dbSet;

    #endregion

    #region Конструктор

    /// <summary>
    /// Создает новый экземпляр класса <see cref="BaseRepository{TEntity, TId, TDbContext}"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если context равен null.</exception>
    protected BaseRepository(TDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получает сущности в коротком формате.
    /// </summary>
    /// <param name="ct">Токен отмены асинхронной операции.</param>
    /// <returns>Список всех сущностей типа <typeparamref name="TEntity"/>.</returns>
    public virtual async Task<List<TEntity>> GetAllShorts(CancellationToken ct) =>
        await _dbSet.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);

    /// <summary>
    /// Получает сущности в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены асинхронной операции.</param>
    /// <returns>Список всех сущностей типа <typeparamref name="TEntity"/>.</returns>
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken ct) =>
        await _dbSet.AsNoTracking().ToListAsync(ct).ConfigureAwait(false);
    /// <summary>
    /// Получает список сущностей, соответствующих заданному условию.
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct) =>
        await _dbSet.Where(filter).ToListAsync(ct).ConfigureAwait(false);

    /// <summary>
    /// Получает сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="ct">Токен отмены асинхронной операции.</param>
    /// <returns>Сущность типа <typeparamref name="TEntity"/> или null, если не найдена.</returns>
    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id), ct).ConfigureAwait(false);

    /// <summary>
    /// Создает новую сущность и сохраняет её в базе данных.
    /// </summary>
    /// <param name="entity">Сущность для создания.</param>
    /// <param name="ct">Токен отмены асинхронной операции.</param>
    /// <returns>Созданная сущность.</returns>
    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct).ConfigureAwait(false);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Обновляет существующую сущность.
    /// </summary>
    /// <param name="entity">Сущность для обновления.</param>
    /// <param name="ct">Токен отмены асинхронной операции.</param>
    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удаляет сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности для удаления.</param>
    /// <param name="ct">Токен отмены асинхронной операции.</param>
    /// <exception cref="ObjectNotFoundException">Выбрасывается, если сущность не найдена.</exception>
    public virtual async Task DeleteByIdAsync(TId id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct).ConfigureAwait(false)
                     ?? throw new ObjectNotFoundException(
                         $"Entity of type {typeof(TEntity).Name} with id {id} is already deleted or does not exist.");

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    #endregion
}
