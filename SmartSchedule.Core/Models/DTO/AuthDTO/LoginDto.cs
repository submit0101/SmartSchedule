using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.AuthDTO;
/// <summary>
/// DTO для авторизации существующего пользователя.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Имя пользователя (логин).
    /// </summary>
    [Required(ErrorMessage = "Логин обязателен")]
    public required string Username { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    public required string Password { get; set; }
}

