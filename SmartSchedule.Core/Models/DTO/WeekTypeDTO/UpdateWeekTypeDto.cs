namespace SmartSchedule.Core.Models.DTO.WeekTypeDTO;
/// <summary>
/// DTO для обновления типа недели..
/// </summary>
public class UpdateWeekTypeDto
{
    /// <summary>
    /// Идентификатор типа недели, который нужно обновить.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Новое название типа недели (если требуется изменить).
    /// </summary>
    /// <example>Нечетная</example>
    public required string Name { get; set; }
}

