using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Application.DTOs.Building;

/// <summary>
/// DTO для обновления информации о здании.
/// </summary>
public class UpdateBuildingDto
{
    /// <summary>
    /// уникальный индентификатор
    /// </summary>
    public required int Id { get; set; }
    /// <summary>
    /// Новое название здания.
    /// </summary>
    [Required(ErrorMessage = "Название здания обязательно.")]
    [MaxLength(50, ErrorMessage = "Название не может превышать 50 символов.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Новый адрес здания.
    /// </summary>
    [Required(ErrorMessage = "Адрес обязателен.")]
    [MaxLength(255, ErrorMessage = "Адрес слишком длинный.")]
    public string Address { get; set; } = string.Empty;
}