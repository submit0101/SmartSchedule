#nullable disable
namespace SmartSchedule.Core.Models.DTO.PosittonDTO;
/// <summary>
/// Ответ с информацией о должности преподавателя.
/// </summary>
public class ResponsePositionDto
{
    /// <summary>
    /// Уникальный идентификатор должности.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название должности.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание должности (необязательное поле).
    /// </summary>
    public string Description { get; set; }
}