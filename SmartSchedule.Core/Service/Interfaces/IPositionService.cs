
using SmartSchedule.Core.Models.DTO.PosittonDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с должностями преподавателей.
/// </summary>
public interface IPositionService
{
    /// <summary>
    /// Получить всех должностей в  коротком виде
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO преподавателей</returns>
    public Task<List<ShortPositionDto>> GetShortAllAsync(CancellationToken ct);
    /// <summary>
    /// Получить список всех должностей в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список должностей</returns>
    Task<List<ResponsePositionDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить должность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о должности</returns>
    Task<ResponsePositionDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новую должность.
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданная должность</returns>
    Task<ResponsePositionDto> CreateAsync(CreatePositionDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию о должности.
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdatePositionDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить должность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);
}