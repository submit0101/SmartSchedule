using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Application.DTOs.Building;

/// <summary>
/// модель ответа
/// </summary>
public class ResponseBuildingDto
{
    /// <summary>
    /// уникальный индентификатор
    /// </summary>
    public required int Id { get; set; }
    /// <summary>
    /// Новое название здания.
    /// </summary>
    [Required(ErrorMessage = "Название здания обязательно.")]
    [MaxLength(100, ErrorMessage = "Название не может превышать 100 символов.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Новый адрес здания.
    /// </summary>
    [Required(ErrorMessage = "Адрес обязателен.")]
    [MaxLength(255, ErrorMessage = "Адрес слишком длинный.")]
    public string Address { get; set; } = string.Empty;
}