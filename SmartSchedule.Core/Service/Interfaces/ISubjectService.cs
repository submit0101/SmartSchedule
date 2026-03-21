
using SmartSchedule.Core.Models.DTO.SubjectDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с учебными дисциплинами (предметами).
/// </summary>
public interface ISubjectService
{
    /// <summary>
    /// Получить список всех предметов в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список предметов</returns>
    Task<List<ResponseSubjectDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить предмет по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о предмете</returns>
    Task<ResponseSubjectDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новый предмет.
    /// </summary>
    /// <param name="dto">Данные для создания предмета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный предмет</returns>
    Task<ResponseSubjectDto> CreateAsync(CreateSubjectDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию о предмете.
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateSubjectDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить предмет по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);
}