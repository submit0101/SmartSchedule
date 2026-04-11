using SmartSchedul.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Service.Interfaces;

/// <summary>
/// Интерфейс сервиса для управления аутентификацией и регистрацией пользователей.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрация нового пользователя в системе.
    /// </summary>
    /// <param name="dto">Данные для регистрации (логин, почта, пароль).</param>
    /// <returns>True, если регистрация прошла успешно.</returns>
    /// <exception cref="BusinessException">Выбрасывается, если логин занят или данные не валидны.</exception>
    Task<bool> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Авторизация пользователя и выдача JWT токена.
    /// </summary>
    /// <param name="dto">Данные для входа (логин и пароль).</param>
    /// <returns>Объект с токеном и информацией о пользователе.</returns>
    /// <exception cref="BusinessException">Выбрасывается при неверных учетных данных.</exception>
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}