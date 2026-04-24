using System.Collections.Generic;

namespace SmartSchedule.Core.Models.DTO.ReportDTO
{
    /// <summary>
    /// DTO, представляющий одну строку (объект группировки) в динамическом отчете.
    /// </summary>
    public class ReportRowDto
    {
        /// <summary>
        /// Название строки (например, ФИО преподавателя, название группы или номер кабинета).
        /// </summary>
        public string RowName { get; set; } = string.Empty;

        /// <summary>
        /// Словарь значений для ячеек строки, где ключ — это название колонки, а значение — количество часов.
        /// Свойство доступно только для чтения.
        /// </summary>
        public Dictionary<string, int> Values { get; } = new Dictionary<string, int>();

        /// <summary>
        /// Суммарное количество часов по всем ячейкам данной строки (итоговая нагрузка).
        /// </summary>
        public int TotalHours { get; set; }
    }
}