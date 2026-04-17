using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.AuthDTO;

/// <summary>
/// DTO для создания нового пользователя администратором.
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Логин нового пользователя.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email нового пользователя.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль нового пользователя.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Роль, которая будет назначена пользователю при создании.
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;
}