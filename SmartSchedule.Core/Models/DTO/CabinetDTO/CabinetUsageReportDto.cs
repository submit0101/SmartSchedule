#nullable disable
namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// DTO для отчёта о загруженности аудитории.
/// Содержит данные о количестве занятий и проценте использования аудитории.
/// </summary>
public class CabinetUsageReportDto
{
    /// <summary>
    /// Номер аудитории.
    /// </summary>
    /// <example>214</example>
    public string CabinetNumber { get; set; }

    /// <summary>
    /// Название здания, в котором находится аудитория.
    /// </summary>
    /// <example>Главный корпус</example>
    public string BuildingName { get; set; }

    /// <summary>
    /// Общее количество занятий, проведённых в этой аудитории за неделю.
    /// </summary>
    /// <example>24</example>
    public int TotalLessons { get; set; }

    /// <summary>
    /// Процент использования аудитории относительно максимально возможного числа занятий.
    /// Рассчитывается как отношение фактических занятий к максимальному числу пар в неделю.
    /// </summary>
    /// <example>66.67</example>
    public double UsagePercentage { get; set; }
}
