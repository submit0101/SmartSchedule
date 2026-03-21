using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.PosittonDTO;
/// <summary>
/// DTO для создания новой должности преподавателя.
/// </summary>
public class CreatePositionDto
{
    /// <summary>
    /// Название должности.
    /// </summary>
    /// <example>Профессор</example>
    [StringLength(30, ErrorMessage = "Список оборудования не должен превышать 30 символов")]
    [RegularExpression(@"^[А-Яа-яЁё\- ]+$",
    ErrorMessage = "Название должно быть только на русском языке (можно с пробелами и дефисами)")]
    public required string Name { get; set; }

    /// <summary>
    /// Описание должности (необязательное поле).
    /// </summary>
    /// <example>Научный руководитель кафедры</example>
    [StringLength(100, ErrorMessage = "Список оборудования не должен превышать 200 символов")]
    public required string Description { get; set; }
}

