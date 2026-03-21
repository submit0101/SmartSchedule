using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// DTO для создания новой аудитории.
/// </summary>
public class CreateCabinetDto
{
    /// <summary>
    /// Номер аудитории.
    /// </summary>
    /// <example>105</example>
    [Required(ErrorMessage = "Номер аудитории обязателен")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Номер аудитории должен содержать от 1 до 10 символов")]
    [RegularExpression(@"^[a-zA-Zа-яА-Я0-9\-]+$", ErrorMessage = "Номер аудитории может содержать только буквы, цифры и дефисы")]
    public required string Number { get; set; }

    /// <summary>
    /// Здание или корпус, в котором находится аудитория.
    /// </summary>
    /// <example>Главный корпус</example>
    public required int BuildingId { get; set; }

    /// <summary>
    /// Вместимость аудитории (может быть null).
    /// </summary>
    /// <example>30</example>
    public int? Capacity { get; set; }

    /// <summary>
    /// Перечень оборудования, доступного в аудитории.
    /// </summary>
    /// <example>Проектор, доска, компьютеры</example>
    [StringLength(200, ErrorMessage = "Список оборудования не должен превышать 200 символов")]
    public string? Equipment { get; set; }
}
