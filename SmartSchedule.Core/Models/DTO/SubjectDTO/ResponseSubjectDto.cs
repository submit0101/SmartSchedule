#nullable disable
namespace SmartSchedule.Core.Models.DTO.SubjectDTO;
/// <summary>
/// Ответ с информацией о предмете.
/// </summary>
public class ResponseSubjectDto
{
    /// <summary>
    /// Уникальный идентификатор предмета.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название предмета.
    /// </summary>
    public string Title { get; set; }
}

