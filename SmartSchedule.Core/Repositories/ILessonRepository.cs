using SmartSchedule.Core.Entities;
using System.Collections.ObjectModel;

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
    Task<HashSet<int>> GetBusyCabinetIdsAsync(int dayOfWeekId, int timeSlotId,int weekTypeId,CancellationToken ct);

    /// <summary>
    /// Получает расписание для отдельного преподавателя по его идентификатору.
    /// </summary>
    /// <param name="teacherId">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список занятий, относящихся к указанному преподавателю.</returns>
    Task<List<Lesson>> GetByTeacherIdAsync(int teacherId, CancellationToken ct = default);
     /// <summary>
    /// Получит информацию о слоте в расписание для каждой группы
    /// </summary>
       /// <param name="dayId">Идентификатор для </param>
       /// <param name="timeId">Идентификатор пары</param>
       /// <param name="ct">Токен отмены операции</param>
       /// <returns></returns>
    Task<List<Lesson>> GetLessonsBySlotAsync(int dayId, int timeId, CancellationToken ct);
    /// <summary>
    /// Пакетное обновление списка занятий в рамках одной транзакции
    /// </summary>
    Task UpdateBatchAsync(IReadOnlyCollection<Lesson> lessons, CancellationToken ct);
    /// <summary>
    /// Получает полный список занятий со всеми связанными сущностями для построения аналитических отчетов.
    /// Выполняется без отслеживания изменений (AsNoTracking) для повышения производительности.
    /// </summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список занятий со связанными данными (Преподаватель, Предмет, Кабинет и т.д.).</returns>
    Task<List<Lesson>> GetLessonsForReportAsync(CancellationToken ct = default);
    /// <summary>
    /// Извлекает матрицу занятости (идентификаторы преподавателей, дней и слотов) для заданных преподавателей и типа недели.
    /// </summary>
    /// <param name="teacherIds">Коллекция идентификаторов преподавателей (только для чтения).</param>
    /// <param name="weekTypeId">Идентификатор типа недели (числитель, знаменатель и т.д.).</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список кортежей, представляющих занятые временные слоты преподавателей.</returns>
    Task<List<(int TeacherId, int DayId, int SlotId)>> GetBusyMatrixAsync(ReadOnlyCollection<int> teacherIds, int weekTypeId, CancellationToken ct = default);
}
