#nullable disable
namespace SmartSchedule.Core.Models.DTO.GroupDTO;
/// <summary>
/// Ответ с информацией о группе.
/// </summary>
public class ResponseGroupDto
{
    /// <summary>
    /// Уникальный идентификатор группы.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название группы.
    /// </summary>
    public string Name { get; set; }
}
