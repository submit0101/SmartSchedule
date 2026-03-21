#nullable disable
namespace SmartSchedule.Core.Models.DTO.LessonDTO;
/// <summary>
/// модель ответа стуртурированного расписания
/// </summary>
public class StructuredLessonDto
{
    /// <summary>
    /// модель ответа 
    /// </summary>
#pragma warning disable CA1002 // Не предоставляйте универсальные списки
#pragma warning disable CA2227 // Свойства коллекций должны быть доступны только для чтения
    public List<ResponseLessonDto> Lessons { get; set; }
#pragma warning restore CA2227 // Свойства коллекций должны быть доступны только для чтения
#pragma warning restore CA1002 // Не предоставляйте универсальные списки
    /// <summary>
    /// показать время
    /// </summary>
    public string TimeSlotDisplay { get; set; }
}
