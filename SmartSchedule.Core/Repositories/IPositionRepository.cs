using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс репозитория Должности
/// </summary>
public interface IPositionRepository : IBaseRepository<Position, int>
{
    /// <summary>
    /// Проверка уникальности кабинета
    /// </summary>
    /// <param name="name">номер кабинета</param>
    /// <param name="excludeId">уникальный иднтификатор записи</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default);
}
