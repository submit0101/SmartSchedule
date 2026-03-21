#nullable disable
namespace SmartSchedule.Core.Models.DTO.PosittonDTO;
/// <summary>
/// Краткая информация о преподавателях
/// </summary>
public class ShortPositionDto
{
    /// <summary>
    /// Уникальный идентификатор преподавателя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название должности.
    /// </summary>
    public string Name { get; set; }
}

