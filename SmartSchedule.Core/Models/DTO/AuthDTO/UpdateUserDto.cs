using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.AuthDTO;

/// <summary>
/// DTO для обновления данных существующего пользователя.
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Новый логин пользователя.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Новый Email пользователя.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}