using SmartSchedule.Core.Models.DTO.TeacherDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с преподавателями.
/// </summary>
public interface ITeacherService
{
    /// <summary>
    /// Получить всех преподавателей в обычном коротком виде
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO преподавателей</returns>
    public Task<List<ShortTeacherDto>> GetShortAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить список всех преподавателей в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список преподавателей</returns>
    Task<List<ResponseTeacherDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить преподавателя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о преподавателе</returns>
    Task<ResponseTeacherDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать нового преподавателя.
    /// </summary>
    /// <param name="dto">Данные для создания преподавателя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный преподаватель</returns>
    Task<ResponseTeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию о преподавателе.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить преподавателя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);

    /// <summary>
    /// Выполняет поиск преподавателей по ФИО и фильтрацию по должности.
    /// </summary>
    /// <param name="search">Поисковая строка (ФИО).</param>
    /// <param name="positionId">Идентификатор должности.</param>
    /// <returns>Список найденных преподавателей.</returns>
    Task<List<ResponseTeacherDto>> SearchTeachersAsync(string? search, int? positionId);
}