#nullable disable
namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// DTO (Data Transfer Object) для представления расписания кабинета по дням недели и временным слотам.
/// </summary>
public class CabinetScheduleReportDto
{
    /// <summary>
    /// Идентификатор дня недели (0 = Sunday, 1 = Monday, ..., 6 = Saturday).
    /// </summary>
    public int DayOfWeekId { get; set; }

    /// <summary>
    /// Название дня недели, полученное на основе DayOfWeekId.
    /// </summary>
    public string DayName { get; set; }

    /// <summary>
    /// Идентификатор временного слота.
    /// </summary>
    public int TimeSlotId { get; set; }

    /// <summary>
    /// Текстовое представление временного слота (например, "08:00 - 09:00").
    /// </summary>
    public string TimeSlotDisplay { get; set; }

    /// <summary>
    /// Флаг, указывающий, занят ли кабинет в указанный день и временной слот.
    /// </summary>
    public bool IsBusy { get; set; }
}


