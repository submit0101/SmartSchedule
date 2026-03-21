using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс репозитория строения
/// </summary>
public interface IBuildingRepository : IBaseRepository<Building, int>
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
