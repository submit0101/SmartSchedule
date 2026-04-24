using System.Collections.Generic;

using System.Collections.ObjectModel;

namespace SmartSchedule.Core.Models.DTO.ReportDTO
{
    /// <summary>
    /// DTO, представляющий готовый результат динамического отчета для отрисовки на клиенте.
    /// </summary>
    public class DynamicReportResultDto
    {
        /// <summary>
        /// Уникальный список заголовков колонок (Ось X).
        /// Свойство доступно только для чтения.
        /// </summary>
        public Collection<string> Columns { get; } = new Collection<string>();

        /// <summary>
        /// Список строк отчета (Ось Y) с вычисленными значениями для каждой ячейки.
        /// Свойство доступно только для чтения.
        /// </summary>
        public Collection<ReportRowDto> Rows { get; } = new Collection<ReportRowDto>();
    }
}