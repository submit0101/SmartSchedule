using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.AuthDTO;

/// <summary>
/// DTO для регистрации нового пользователя.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Имя пользователя (логин) для входа в систему.
    /// </summary>
    [Required(ErrorMessage = "Логин обязателен")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен содержать от 3 до 50 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Логин может содержать только латинские буквы, цифры и знак подчеркивания")]
    public required string Username { get; set; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат электронной почты")]
    public required string Email { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public required string Password { get; set; }
}
