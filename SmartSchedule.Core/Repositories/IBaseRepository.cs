using SmartSchedule.Core.Entities.Abstractions;
using System.Linq.Expressions;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс Ьазового репозитория
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TId"></typeparam>
public interface IBaseRepository<TEntity, in TId> where TEntity : class, IEntity<TId>
{   /// <summary>
    /// Возращает каротки список всех сущностей
    /// </summary>
    /// <param name="ct">Токен отмены операции</param>
    /// <returns></returns>
    Task<List<TEntity>> GetAllShorts(CancellationToken ct);
    /// <summary>
    /// получить все значения
    /// </summary>
    /// <param name="filter">фильтер</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, CancellationToken ct);

    /// <summary>
    /// Возвращает список всех сущностей.
    /// </summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список сущностей.</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Возвращает сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Сущность.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct);

    /// <summary>
    /// Добавляет новую сущность.
    /// </summary>
    /// <param name="entity">Данные для создания.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданная сущность.</returns>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Обновляет сущность.
    /// </summary>
    /// <param name="entity">Данные для обновления.</param>
    /// <param name="ct">Токен отмены операции.</param>
    Task UpdateAsync(TEntity entity, CancellationToken ct);

    /// <summary>
    /// Удаляет сущность по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="ct">Токен отмены операции.</param>
    Task DeleteByIdAsync(TId id, CancellationToken ct);

}
