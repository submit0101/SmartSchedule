using SmartSchedule.Core.Models.DTO.TeacherDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с преподавателями.
/// </summary>
public interface ITeacherService
{
    /// <summary>
    /// Получает всех преподавателей в коротком формате.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список коротких DTO преподавателей.</returns>
    Task<List<ShortTeacherDto>> GetShortAllAsync(CancellationToken ct);

    /// <summary>
    /// Получает список всех преподавателей.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список полных DTO преподавателей.</returns>
    Task<List<ResponseTeacherDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получает преподавателя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO преподавателя.</returns>
    Task<ResponseTeacherDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создает нового преподавателя.
    /// </summary>
    /// <param name="dto">Данные для создания.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO созданного преподавателя.</returns>
    Task<ResponseTeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken ct);

    /// <summary>
    /// Обновляет данные преподавателя.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="dto">Данные для обновления.</param>
    /// <param name="ct">Токен отмены.</param>
    Task UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken ct);

    /// <summary>
    /// Удаляет преподавателя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены.</param>
    Task DeleteAsync(int id, CancellationToken ct);

    /// <summary>
    /// Выполняет поиск преподавателей по ФИО.
    /// </summary>
    /// <param name="search">Строка поиска.</param>
    /// <returns>Список найденных преподавателей.</returns>
    Task<List<ResponseTeacherDto>> SearchTeachersAsync(string? search);

    /// <summary>
    /// Импортирует преподавателей из потока Excel-файла.
    /// </summary>
    /// <returns>Количество успешно добавленных записей.</returns>
    Task<int> ImportFromExcelAsync(Stream excelStream, CancellationToken ct);
}