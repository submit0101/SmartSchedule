using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTOs.Building;

/// <summary>
/// DTO для создания нового здания.
/// </summary>
public class CreateBuildingDto
{
    /// <summary>
    /// Название здания (например, "Старый корпус").
    /// </summary>
    [Required(ErrorMessage = "Название здания обязательно.")]
    [MaxLength(50, ErrorMessage = "Название не может превышать 50 символов.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Адрес здания.
    /// </summary>
    [Required(ErrorMessage = "Адрес обязателен.")]
    [MaxLength(255, ErrorMessage = "Адрес слишком длинный.")]
    public string Address { get; set; } = string.Empty;
}

