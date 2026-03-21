using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// DTO для обновления аудитории.
/// </summary>
public class UpdateCabinetDto
{
    /// <summary>
    /// Идентификатор аудитории, которую нужно обновить.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Новый номер аудитории (если требуется изменить).
    /// </summary>
    /// <example>106</example>
    [Required(ErrorMessage = "Номер аудитории обязателен")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Номер аудитории должен содержать от 1 до 10 символов")]
    [RegularExpression(@"^[a-zA-Zа-яА-Я0-9\-]+$", ErrorMessage = "Номер аудитории может содержать только буквы, цифры и дефисы")]
    public required string Number { get; set; }

    /// <summary>
    /// Новое здание или корпус (если требуется изменить).
    /// </summary>
    /// <example>Новый корпус</example>
    public required int BuildingId { get; set; }

    /// <summary>
    /// Новая вместимость аудитории (может быть null).
    /// </summary>
    /// <example>25</example>
    public int? Capacity { get; set; }

    /// <summary>
    /// Новое оборудование аудитории.
    /// </summary>
    /// <example>Проектор, ноутбук</example>
    [StringLength(200, ErrorMessage = "Список оборудования не должен превышать 200 символов")]
    public string? Equipment { get; set; }
}
