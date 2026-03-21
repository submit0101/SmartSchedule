
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с типами недель.
/// </summary>
public interface IWeekTypeService
{

    /// <summary>
    /// Получить список всех типов недель в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список типов недель</returns>
    Task<List<ResponseWeekTypeDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить тип недели по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о типе недели</returns>
    Task<ResponseWeekTypeDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новый тип недели.
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный тип недели</returns>
    Task<ResponseWeekTypeDto> CreateAsync(CreateWeekTypeDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить тип недели.
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateWeekTypeDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить тип недели по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);
}