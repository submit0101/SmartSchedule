namespace SmartSchedule.Core.Models.DTO.LessonDTO;
/// <summary>
/// DTO для создания нового занятия.
/// </summary>
public class CreateLessonDto
{
    /// <summary>
    /// Идентификатор аудитории, в которой проводится занятие (может быть null).
    /// </summary>
    public required int? CabinetId { get; set; }

    /// <summary>
    /// Идентификатор преподавателя, который проводит занятие (может быть null).
    /// </summary>
    public required int? TeacherId { get; set; }

    /// <summary>
    /// Номер подгруппы (например, 1 или 2). Может быть null (вся группа).
    /// </summary>
    public int? Subgroup { get; set; }

    /// <summary>
    /// Идентификатор группы, для которой проводится занятие (может быть null).
    /// </summary>
    public required int? GroupId { get; set; }

    /// <summary>
    /// Идентификатор предмета, который изучается на занятии (может быть null).
    /// </summary>
    public required int? SubjectId { get; set; }

    /// <summary>
    /// Идентификатор временного слота, в котором проходит занятие (может быть null).
    /// </summary>
    public required int? TimeSlotId { get; set; }

    /// <summary>
    /// Идентификатор типа недели, к которому относится занятие (может быть null).
    /// </summary>
    public required int? WeekTypeId { get; set; }

    /// <summary>
    /// День недели, в который проводится занятие (например: 1 - Понедельник, 5 - Пятница).
    /// </summary>
    /// <example>2</example>
    public required int DayOfWeekId { get; set; }
}

