
#nullable disable
namespace SmartSchedule.Core.Models.DTO.LessonDTO;
/// <summary>
/// Ответ с информацией о занятии.
/// Содержит связанные данные для отображения.
/// </summary>
public class ResponseLessonDto
{
    /// <summary>
    /// Уникальный идентификатор занятия.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор аудитории.
    /// </summary>
    public int? CabinetId { get; set; }

    /// <summary>
    /// Номер аудитории.
    /// </summary>
    public string CabinetNumber { get; set; }

    /// <summary>
    /// Идентификатор преподавателя.
    /// </summary>
    public int? TeacherId { get; set; }

    /// <summary>
    /// ФИО преподавателя.
    /// </summary>
    public string TeacherFullName { get; set; }

    /// <summary>
    /// Идентификатор группы.
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// Название группы.
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// Идентификатор предмета.
    /// </summary>
    public int? SubjectId { get; set; }

    /// <summary>
    /// Название предмета.
    /// </summary>
    public string SubjectTitle { get; set; }

    /// <summary>
    /// Идентификатор временного слота.
    /// </summary>
    public int? TimeSlotId { get; set; }

    /// <summary>
    /// Порядковый номер временного слота.
    /// </summary>
    public int TimeSlotNumber { get; set; }

    /// <summary>
    /// Название корпуса, в котором находится аудитория.
    /// </summary>
    public string BuildingName { get; set; }

    /// <summary>
    /// Время начала и окончания занятия (формат: "08:30 - 10:00").
    /// </summary>
    public string TimeSlotDisplay { get; set; }

    /// <summary>
    /// Идентификатор типа недели.
    /// </summary>
    public int? WeekTypeId { get; set; }

    /// <summary>
    /// Название типа недели.
    /// </summary>
    public string WeekTypeName { get; set; }

    /// <summary>
    /// День недели (например: 1 - Понедельник).
    /// </summary>
    public int DayOfWeekId { get; set; }

    /// <summary>
    /// Номер подгруппы (например, 1 или 2). Может быть null (вся группа).
    /// </summary>
    public int? Subgroup { get; set; }
}
