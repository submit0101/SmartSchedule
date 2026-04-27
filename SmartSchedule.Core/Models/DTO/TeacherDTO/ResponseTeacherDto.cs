namespace SmartSchedule.Core.Models.DTO.TeacherDTO;

#nullable enable

/// <summary>
/// Ответ с информацией о преподавателе.
/// </summary>
public class ResponseTeacherDto
{
    /// <summary>
    /// Уникальный идентификатор преподавателя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя преподавателя.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Отчество преподавателя.
    /// </summary>
    /// <remarks>Может быть пустым.</remarks>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Фамилия преподавателя.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Полное имя преподавателя (Фамилия Имя Отчество).
    /// </summary>
    public required string FullName { get; set; }
}