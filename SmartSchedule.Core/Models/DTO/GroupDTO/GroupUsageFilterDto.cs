using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.GroupDTO;
/// <summary>
/// принимающие данные 
/// </summary>
public class GroupUsageFilterDto
{
    /// <summary> Идентификатор дня недели (1-7), по которому нужно посчитать загрузку. Null - для всей недели. </summary>
    public int? DayOfWeekId { get; set; }

    /// <summary> Тип недели (1 - Нечетная, 2 - Четная, 3 - Целая, 0/Null - для всех недель). </summary>
    public int? WeekTypeId { get; set; }
}
