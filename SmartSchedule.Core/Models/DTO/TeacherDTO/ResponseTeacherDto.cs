#nullable disable
namespace SmartSchedule.Core.Models.DTO.TeacherDTO;
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
    public string FirstName { get; set; }

    /// <summary>
    /// Отчество преподавателя.
    /// </summary>
    public string MiddleName { get; set; }

    /// <summary>
    /// Фамилия преподавателя.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Полное имя преподавателя (Фамилия Имя Отчество).
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Название должности преподавателя.
    /// </summary>
    public string PositionName { get; set; }
}
