using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс репозитория Группы
/// </summary>
public interface IGroupRepository : IBaseRepository<Group, int>
{
    /// <summary>
    /// Проверка уникальности кабинета
    /// </summary>
    /// <param name="name">номер кабинета</param>
    /// <param name="excludeId">уникальный иднтификатор записи</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<bool> ExistsByAsync(string name, int? excludeId = null, CancellationToken ct = default);

}
