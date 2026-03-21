using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Repositories;
/// <summary>
/// Интерфейс репозитория Занятия
/// </summary>
public interface ILessonRepository : IBaseRepository<Lesson, int>
{
    /// <summary>
    /// получить рассписание для определенной группы
    /// </summary>
    /// <param name="groupId">группа</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<List<Lesson>> GetByGroupIdAsync(int groupId, CancellationToken ct);
    /// <summary>
    /// Фильтер уроков
    /// </summary>
    /// <param name="weekTypeId">тип недели</param>
    /// <param name="dayOfWeekId">день недели</param>
    /// <param name="cabinetId">кабинет </param>
    /// <param name="ct"> токен отмены</param>
    /// <returns></returns>
    Task<List<Lesson>> GetFilteredLessonsAsync(
    int? weekTypeId,
    int? dayOfWeekId,
    int? cabinetId,
    CancellationToken ct);
    /// <summary>
    /// Получает ID кабинетов, занятых в указанное время/день/неделю.
    /// </summary>
    Task<HashSet<int>> GetBusyCabinetIdsAsync(
        int dayOfWeekId,
        int timeSlotId,
        int weekTypeId,
        CancellationToken ct);
    /// <summary>
    /// Получает расписание для отдельного преподавателя по его идентификатору.
    /// </summary>
    /// <param name="teacherId">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список занятий, относящихся к указанному преподавателю.</returns>
    Task<List<Lesson>> GetByTeacherIdAsync(int teacherId, CancellationToken ct = default);


}
