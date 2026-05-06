using System.Collections.ObjectModel;

namespace SmartSchedule.Core.Models.DTO.TeacherDTO;

/// <summary>
/// Результат импорта преподавателей.
/// </summary>
public class ImportResultDto
{
    /// <summary>
    /// Количество успешно добавленных преподавателей.
    /// </summary>
    public int AddedCount { get; set; }

    /// <summary>
    /// Список ошибок при импорте.
    /// </summary>
    public Collection<string> Errors { get; } = new();
}