#nullable disable
namespace SmartSchedule.Core.Models.DTO.BuildingDTO;
/// <summary>
/// Модель краткого ответа
/// </summary>
public class ShortBuildingDto
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Имя корпуса
    /// </summary>
    public string Name { get; set; }
}