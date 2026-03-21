#nullable disable
namespace SmartSchedule.Core.Models.DTO.WeekTypeDTO;
/// <summary>
/// Ответ с информацией о типе недели.
/// </summary>
public class ResponseWeekTypeDto
{
    /// <summary>
    /// Уникальный идентификатор типа недели.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название типа недели.
    /// </summary>
    public string Name { get; set; }
}
