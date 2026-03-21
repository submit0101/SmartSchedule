using SmartSchedule.Core.Models.DTO.CabinetDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с аудиториями.
/// </summary>
public interface ICabinetService
{
    /// <summary>
    /// Получить список всех аудиторий в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список аудиторий</returns>
    Task<List<ResponseCabinetDto>> GetAllAsync(CancellationToken ct);
    /// <summary>
    /// получить список кабинетов в коротком формате с всязкой
    /// </summary>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<List<ShortCabinetDto>> GetAllShortWithBuilding(CancellationToken ct);

    /// <summary>
    /// Получить аудиторию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор аудитории</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о аудитории</returns>
    Task<ResponseCabinetDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новую аудиторию.
    /// </summary>
    /// <param name="dto">Данные для создания аудитории</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданная аудитория</returns>
    Task<ResponseCabinetDto> CreateAsync(CreateCabinetDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию об аудитории.
    /// </summary>
    /// <param name="id">Идентификатор аудитории</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateCabinetDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить аудиторию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор аудитории</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);

    /// <summary>
    /// Поиск кабинетов по номеру и/или зданию с сортировкой.
    /// </summary>
    /// <param name="searchTerm">Числовой номер кабинета (например "310")</param>
    /// <param name="buildingNumber">Номер здания (1 или 2)</param>
    /// <param name="sortBy">Поле для сортировки ("number", "building")</param>
    /// <param name="descending">Направление сортировки (true - по убыванию)</param>
    /// <returns>Список подходящих кабинетов</returns>
    Task<List<ResponseCabinetDto>> SearchCabinetsAsync(
        string? searchTerm,
        int? buildingNumber,
        string? sortBy,
        bool descending);


}