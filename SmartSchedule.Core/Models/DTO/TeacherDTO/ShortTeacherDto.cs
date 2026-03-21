#nullable disable
namespace SmartSchedule.Core.Models.DTO.TeacherDTO;
/// <summary>
/// Краткая информация о преподавателе.
/// Используется для минимизации объема передаваемых данных в ответах API.
/// </summary>
public class ShortTeacherDto
{
    /// <summary>
    /// Уникальный идентификатор преподавателя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Полное имя преподавателя в формате "Фамилия Имя Отчество".
    /// Это поле вычисляется на основе отдельных полей из базы данных: 
    /// Фамилия (Surname), Имя (Name), Отчество (Patronymic).
    /// Не хранится напрямую в БД.
    /// </summary>
    public string FullName { get; set; }
}

