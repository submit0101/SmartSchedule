using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.TeacherDTO;

/// <summary>
/// DTO для вывода статистики по загруженности преподавателя.
/// </summary>
public class TeacherUsageReportDto
{
    /// <summary>
    /// Полное имя преподавателя (ФИО).
    /// </summary>
    public required string TeacherFullName { get; set; }

    /// <summary>
    /// Общее количество назначенных занятий (количество записей в расписании), найденных по заданным фильтрам.
    /// </summary>
    public int TotalLessons { get; set; }
    /// <summary>
    /// Максимально возможное количество слотов (знаменатель) в отчетном периоде.
    /// </summary>
    public int MaxPossibleLessons { get; set; }

    /// <summary>
    /// Процент использования временных слотов (загруженности) относительно максимального количества слотов в отчетном периоде.
    /// </summary>
    public double UsagePercentage { get; set; }
}
