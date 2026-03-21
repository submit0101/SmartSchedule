using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс репозитория Кабинета
/// </summary>
public interface ICabinetRepository : IBaseRepository<Cabinet, int>
{
    /// <summary>
    /// Асинхронно получает все кабинеты,включая связанные строения
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<List<Cabinet>> GetAllWithBuldingAsync(CancellationToken ct);

    /// <summary>
    /// Поиск кабинетов по номеру и/или зданию с сортировкой.
    /// </summary>
    /// <param name="searchTerm">Числовой номер кабинета (например "310")</param>
    /// <param name="buildingNumber">Номер здания (1 или 2)</param>
    /// <param name="sortBy">Поле для сортировки ("number", "building")</param>
    /// <param name="descending">Направление сортировки (true - по убыванию)</param>
    /// <returns>Список подходящих кабинетов</returns>
    Task<List<Cabinet>> SearchAsync(
            string? searchTerm,
            int? buildingNumber,
            string? sortBy,
            bool descending);
    /// <summary>
    /// Фильтер кабинетов
    /// </summary>
    /// <param name="buildingId">Уникальный идентификатор здания</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<List<Cabinet>> GetAllFilteredAsync(int? buildingId, CancellationToken ct);
    /// <summary>
    /// получить все
    /// </summary>
    /// <param name="cabinetId">получить кабинет по id</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<Cabinet> GetWithLessonsByIdAsync(int cabinetId, CancellationToken ct);
    /// <summary>
    /// Проверка уникальности кабинета
    /// </summary>
    /// <param name="number">номер кабинета</param>
    /// <param name="buildingId">номер корпуса</param>
    /// <param name="excludeId">уникальный иднтификатор записи</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string number, int buildingId, int? excludeId = null, CancellationToken ct = default);

}
