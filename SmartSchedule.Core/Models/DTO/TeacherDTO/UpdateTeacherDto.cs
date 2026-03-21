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
    /// Новое имя преподавателя (если требуется изменить).
    /// </summary>
    /// <example>Петр</example>
    public required string FirstName { get; set; }

    /// <summary>
    /// Новое отчество преподавателя (если требуется изменить).
    /// </summary>
    /// <example>Петрович</example>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Новая фамилия преподавателя (если требуется изменить).
    /// </summary>
    /// <example>Петров</example>
    public required string LastName { get; set; }

    /// <summary>
    /// Новый идентификатор должности преподавателя (если требуется изменить).
    /// </summary>
    /// <example>2</example>
    public int PositionId { get; set; }
}


