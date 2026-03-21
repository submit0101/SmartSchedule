using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.TeacherDTO;

/// <summary>
/// Фильтр для отчета о загруженности преподавателей.
/// </summary>
public class TeacherUsageFilterDto
{
    /// <summary>
    /// Идентификатор дня недели (1-6). Если <see langword="null"/>, учитываются все дни.
    /// </summary>
    public int? DayOfWeekId { get; set; }

    /// <summary>
    /// Идентификатор типа недели (1 - нечетная, 2 - четная, 3 - обе). Если <see langword="null"/>, учитываются все типы недель.
    /// </summary>
    public int? WeekTypeId { get; set; }
}
