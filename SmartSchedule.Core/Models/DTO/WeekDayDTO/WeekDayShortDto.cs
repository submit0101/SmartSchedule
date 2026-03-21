#nullable disable
namespace SmartSchedule.Application.DTOs
{
    /// <summary>
    ///  краткая модель ответа для выпадашки
    /// </summary>
    public class WeekDayShortDto
    {
        /// <summary>
        /// Уникальный индетификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// имя для дня недели
        /// </summary>
        public string Name { get; set; }
    }
}

