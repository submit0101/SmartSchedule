namespace SmartSchedule.Core.Models.DTO.TeacherDTO;

/// <summary>
/// DTO для обновления преподавателя.
/// </summary>
public class UpdateTeacherDto
{
    /// <summary>
    /// Идентификатор преподавателя, которого нужно обновить.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Новое имя преподавателя.
    /// </summary>
    /// <example>Петр</example>
    public required string FirstName { get; set; }

    /// <summary>
    /// Новое отчество преподавателя.
    /// </summary>
    /// <example>Петрович</example>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Новая фамилия преподавателя.
    /// </summary>
    /// <example>Петров</example>
    public required string LastName { get; set; }
}