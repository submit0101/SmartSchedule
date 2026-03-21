using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.GroupDTO;
/// <summary>
/// dto для получение структурированного отчёта
/// </summary>
public class GroupUsageReportDto
{
    /// <summary> Название группы (например, "22-ПРО-2") </summary>
    public required string GroupName { get; set; }

    /// <summary> Фактически запланировано занятий за период </summary>
    public int TotalScheduledLessons { get; set; }

    /// <summary> Максимально возможное количество пар (6 дней * MaxTimeSlots) </summary>
    public int MaxPossibleLessons { get; set; }

    /// <summary> Процент занятости (TotalScheduled / MaxPossible * 100) </summary>
    public decimal UsagePercentage { get; set; }
}

