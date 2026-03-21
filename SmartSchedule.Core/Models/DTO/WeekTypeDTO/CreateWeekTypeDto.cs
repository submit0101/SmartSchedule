namespace SmartSchedule.Core.Models.DTO.WeekTypeDTO;
/// <summary>
/// DTO для создания типа недели.
/// </summary>
public class CreateWeekTypeDto
{
    /// <summary>
    /// Название типа недели.
    /// </summary>
    /// <example>Четная</example>
    public required string Name { get; set; }
}


