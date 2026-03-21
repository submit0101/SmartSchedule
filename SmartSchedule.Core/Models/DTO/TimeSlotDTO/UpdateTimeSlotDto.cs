namespace SmartSchedule.Core.Models.DTO.TimeSlotDTO;
/// <summary>
/// DTO для обновления временного слота.
/// </summary>
public class UpdateTimeSlotDto
{
    /// <summary>
    /// Идентификатор временного слота, который нужно обновить.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Новый порядковый номер временного слота (если требуется изменить).
    /// </summary>
    /// <example>2</example>
    public int? SlotNumber { get; set; }

    /// <summary>
    /// Новое время начала занятия (если требуется изменить).
    /// </summary>
    /// <example>10:10</example>
    public TimeOnly? StartTime { get; set; }

    /// <summary>
    /// Новое время окончания занятия (если требуется изменить).
    /// </summary>
    /// <example>11:40</example>
    public TimeOnly? EndTime { get; set; }
}

