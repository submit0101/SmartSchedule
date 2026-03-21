namespace SmartSchedule.Core.Models.DTO.TimeSlotDTO;
/// <summary>
/// Ответ с информацией о временном слоте.
/// </summary>
public class ResponseTimeSlotDto
{
    /// <summary>
    /// Уникальный идентификатор временного слота.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Порядковый номер временного слота.
    /// </summary>
    public int SlotNumber { get; set; }

    /// <summary>
    /// Время начала занятия.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Время окончания занятия.
    /// </summary>
    public TimeOnly EndTime { get; set; }
}

