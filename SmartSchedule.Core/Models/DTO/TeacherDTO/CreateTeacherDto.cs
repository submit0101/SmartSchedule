namespace SmartSchedule.Core.Models.DTO.TeacherDTO;
/// <summary>
/// DTO для создания преподавателя.
/// </summary>
public class CreateTeacherDto
{
    /// <summary>
    /// Имя преподавателя.
    /// </summary>
    /// <example>Иван</example>
    public required string FirstName { get; set; }

    /// <summary>
    /// Отчество преподавателя.
    /// </summary>
    /// <example>Иванович</example>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Фамилия преподавателя.
    /// </summary>
    /// <example>Иванов</example>
    public required string LastName { get; set; }

    /// <summary>
    /// Идентификатор должности преподавателя.
    /// </summary>
    /// <example>1</example>
    public required int PositionId { get; set; }
}