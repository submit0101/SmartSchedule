using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.TeacherDTO;
/// <summary>
/// DTO для представления отдельного временного слота в "тепловой карте" расписания преподавателя.
/// </summary>
public class TeacherScheduleReportDto
{
    /// <summary>
    /// Идентификатор дня недели (1 = Понедельник).
    /// </summary>
    public int DayOfWeekId { get; set; }

    /// <summary>
    /// Название дня недели (например, "Понедельник").
    /// </summary>
    public required string DayName { get; set; }

    /// <summary>
    /// Идентификатор временного слота (пары).
    /// </summary>
    public int TimeSlotId { get; set; }

    /// <summary>
    /// Отображаемый интервал времени для слота (например, "8:30 - 10:00").
    /// </summary>
    public required string TimeSlotDisplay { get; set; }

    /// <summary>
    /// Флаг, указывающий, занят ли преподаватель в данном слоте (<see langword="true"/>) или свободен (<see langword="false"/>).
    /// </summary>
    public bool IsBusy { get; set; }
}
