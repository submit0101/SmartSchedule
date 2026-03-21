
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с временными слотами.
/// </summary>
public interface ITimeSlotService
{

    /// <summary>
    /// Получить список временных слотов в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список временных слотов</returns>
    Task<List<ResponseTimeSlotDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить временной слот по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор слота</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о временном слоте</returns>
    Task<ResponseTimeSlotDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новый временной слот.
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный временной слот</returns>
    Task<ResponseTimeSlotDto> CreateAsync(CreateTimeSlotDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить данные временного слота.
    /// </summary>
    /// <param name="id">Идентификатор слота</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateTimeSlotDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить временной слот по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор слота</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);
}