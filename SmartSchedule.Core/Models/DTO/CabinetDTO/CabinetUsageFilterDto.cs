namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// пробую фильт панель через dto
/// </summary>
public class CabinetUsageFilterDto
{
    /// <summary>
    /// id здания
    /// </summary>
    public int? BuildingId { get; set; }
    /// <summary>
    /// Уникальный Идентификатор типа недели
    /// </summary>
    public int? WeekTypeId { get; set; }
    /// <summary>
    /// Уникальный идетификатор
    /// </summary>
    public int? DayOfWeekId { get; set; }
}

