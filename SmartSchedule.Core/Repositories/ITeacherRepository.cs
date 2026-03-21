using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс репозитория Преподавателя
/// </summary>
public interface ITeacherRepository : IBaseRepository<Teacher, int>
{
    /// <summary>
    /// Ансихрнонно получает всех учителй включая связанные позиции
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    Task<List<Teacher>> GetAllWithPositonAsync(CancellationToken ct);

    /// <summary>
    /// Получить список преподавателей по заданному поисковому запросу.
    /// </summary>
    /// <param name="search">Поисковая строка (ФИО или часть ФИО).</param>
    /// <param name="positionValue">Должность для фильтрации.</param>
    /// <returns>Список преподавателей.</returns>
    Task<List<Teacher>> SearchAsync(string? search, int? positionValue);
    /// <summary>
    /// Получить преподавателя по ID с присоединенными (Include) занятиями.
    /// Необходимо для построения тепловой карты.
    /// </summary>
    /// <param name="id">ID преподавателя</param>
    /// <param name="ct">Токе отмены</param>
    Task<Teacher?> GetWithLessonsByIdAsync(int id, CancellationToken ct = default);

}