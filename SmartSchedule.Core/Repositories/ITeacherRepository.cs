using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;

/// <summary>
/// Интерфейс репозитория Преподавателя.
/// </summary>
public interface ITeacherRepository : IBaseRepository<Teacher, int>
{
    

    /// <summary>
    /// Выполняет поиск преподавателей по заданной строке (ФИО).
    /// </summary>
    /// <param name="search">Поисковая строка.</param>
    /// <returns>Список найденных преподавателей.</returns>
    Task<List<Teacher>> SearchAsync(string? search);

    /// <summary>
    /// Получает преподавателя по идентификатору вместе со связанными занятиями.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Объект преподавателя или null.</returns>
    Task<Teacher?> GetWithLessonsByIdAsync(int id, CancellationToken ct = default);


}