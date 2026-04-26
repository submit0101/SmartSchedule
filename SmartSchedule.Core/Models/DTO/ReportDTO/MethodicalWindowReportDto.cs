using System;
using System.Collections.ObjectModel;

namespace SmartSchedule.Core.Models.DTO.ReportDTO;

/// <summary>
/// Официальный бланк ведомости методических окон для планирования совещаний.
/// </summary>
public class MethodicalWindowReportDto
{
    /// <summary> Заголовок отчета. </summary>
    public string ReportTitle { get; set; } = "Сводная ведомость методических окон (для совещаний)";

    /// <summary> Дата формирования. </summary>
    public DateTime GeneratedDate { get; set; } = DateTime.Now;

    /// <summary> Итоговое количество найденных окон (Суммирование). </summary>
    public int TotalWindowsFound { get; set; }

    /// <summary> 
    /// Список строк отчета. 
    /// Свойство доступно только для чтения (get-only), что защищает коллекцию от случайной перезаписи.
    /// </summary>
    public Collection<MethodicalWindowRowDto> Rows { get; } = new Collection<MethodicalWindowRowDto>();
}