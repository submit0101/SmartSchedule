using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.PosittonDTO;
/// <summary>
/// DTO для обновления должности преподавателя.
/// </summary>
public class UpdatePositionDto
{
    /// <summary>
    /// Идентификатор должности, которую нужно обновить.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Новое название должности (если требуется изменить).
    /// </summary>
    /// <example>Доцент</example>
    [RegularExpression(@"^[А-Яа-яЁё\- ]+$",
    ErrorMessage = "Название должно быть только на русском языке (можно с пробелами и дефисами)")]
    public required string Name { get; set; }

    /// <summary>
    /// Новое описание должности (если требуется изменить).
    /// </summary>
    /// <example>Ведущий преподаватель</example>
    [StringLength(100, ErrorMessage = "Список оборудования не должен превышать 200 символов")]
    public required string Description { get; set; }
}