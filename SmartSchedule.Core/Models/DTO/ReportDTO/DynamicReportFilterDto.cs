namespace SmartSchedule.Core.Models.DTO.ReportDTO
{
    /// <summary>
    /// DTO для фильтрации и настройки динамического отчета (Cross-tab).
    /// </summary>
    public class DynamicReportFilterDto
    {
        /// <summary>
        /// Поле, по которому будет происходить группировка в строках (Ось Y).
        /// По умолчанию - "Teacher" (Преподаватель).
        /// </summary>
        public string RowGrouping { get; set; } = "Teacher";

        /// <summary>
        /// Поле, по которому будет происходить группировка в колонках (Ось X).
        /// По умолчанию - "Subject" (Предмет).
        /// </summary>
        public string ColGrouping { get; set; } = "Subject";
    }
}