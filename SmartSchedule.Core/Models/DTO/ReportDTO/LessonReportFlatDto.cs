namespace SmartSchedule.Core.Models.DTO.ReportDTO;

/// <summary>
/// Вспомогательный DTO для промежуточного представления данных занятия в отчетах.
/// </summary>
public class LessonReportFlatDto
{
    /// <summary>
    /// Полное имя преподавателя.
    /// </summary>
    public string Teacher { get; set; } = string.Empty;

    /// <summary>
    /// Название группы.
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Название предмета (Title).
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Номер кабинета.
    /// </summary>
    public string Cabinet { get; set; } = string.Empty;

    /// <summary>
    /// Название корпуса.
    /// </summary>
    public string Building { get; set; } = string.Empty;
}