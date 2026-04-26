using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.ReportDTO;
/// <summary>
/// Представляет строку ведомости методических окон с информацией о свободном времени.
/// </summary>
public class MethodicalWindowRowDto
{
    /// <summary> Название дня недели. </summary>
    public string DayName { get; set; } = string.Empty;

    /// <summary> Отображение времени (например, "08:00 - 09:30"). </summary>
    public string TimeDisplay { get; set; } = string.Empty;

    /// <summary> Количество свободных преподавателей в этом слоте (Агрегация). </summary>
    public int FreeTeachersCount { get; set; }

    /// <summary> Список имен свободных преподавателей. </summary>
    public string FreeTeachersNames { get; set; } = string.Empty;
}
