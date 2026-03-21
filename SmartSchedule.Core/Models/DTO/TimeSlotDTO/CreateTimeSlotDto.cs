namespace SmartSchedule.Core.Models.DTO.TimeSlotDTO;
/// <summary>
/// DTO для создания временного слота.
/// </summary>
public class CreateTimeSlotDto
{
    /// <summary>
    /// Порядковый номер временного слота.
    /// </summary>
    /// <example>1</example>
    public int SlotNumber { get; set; }

    /// <summary>
    /// Время начала занятия.
    /// </summary>
    /// <example>08:30</example>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Время окончания занятия.
    /// </summary>
    /// <example>10:00</example>
    public TimeOnly EndTime { get; set; }
}

