namespace SmartSchedule.Core.Models.DTO.LessonDTO;
/// <summary>
/// DTO для обновления существующего занятия.
/// </summary>
public class UpdateLessonDto
{
    /// <summary>
    /// Идентификатор занятия, которое нужно обновить.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Новый идентификатор аудитории (если требуется изменить).
    /// </summary>
    public required int? CabinetId { get; set; }

    /// <summary>
    /// Новый идентификатор преподавателя (если требуется изменить).
    /// </summary>
    public required int? TeacherId { get; set; }

    /// <summary>
    /// Новый идентификатор группы (если требуется изменить).
    /// </summary>
    public required int? GroupId { get; set; }

    /// <summary>
    /// Новый идентификатор предмета (если требуется изменить).
    /// </summary>
    public required int? SubjectId { get; set; }

    /// <summary>
    /// Новый идентификатор временного слота (если требуется изменить).
    /// </summary>
    public required int? TimeSlotId { get; set; }

    /// <summary>
    /// Новый идентификатор типа недели (если требуется изменить).
    /// </summary>
    public required int? WeekTypeId { get; set; }

    /// <summary>
    /// Новое значение дня недели (если требуется изменить).
    /// Например: 3 - Среда.
    /// </summary>
    public required int? DayOfWeekId { get; set; }
}

