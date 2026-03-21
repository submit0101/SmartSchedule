namespace SmartSchedule.Core.Models.DTO.LessonDTO;

/// <summary>
/// Результат проверки конфликта для клиента (для предварительной подсветки DnD).
/// </summary>
public class ConflictCheckResultDto
{
    /// <summary>
    /// Конфликт с другими занятиями ТОЙ ЖЕ группы в целевом слоте, 
    /// основанный на Типе Недели (Числитель/Знаменатель).
    /// </summary>
    public bool IsWeekConflict { get; set; }

    /// <summary>
    /// Конфликт по Преподавателю (занят в целевом слоте у ЛЮБОЙ группы).
    /// </summary>
    public bool IsTeacherBusy { get; set; }

    /// <summary>
    /// Конфликт по Кабинету (занят в целевом слоте у ЛЮБОЙ группы).
    /// </summary>
    public bool IsCabinetBusy { get; set; }
}