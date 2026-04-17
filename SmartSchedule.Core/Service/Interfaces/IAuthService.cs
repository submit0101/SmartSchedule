using SmartSchedul.Core.Exceptions;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using System.Collections.Generic;
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

    /// <summary>
    /// Получение списка всех пользователей в системе.
    /// </summary>
    /// <returns>Список пользователей с их ролями.</returns>
    Task<List<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// Назначение новой роли пользователю.
    /// </summary>
    /// <param name="dto">Данные для назначения роли (ID пользователя и название роли).</param>
    /// <returns>True, если роль успешно назначена.</returns>
    /// <exception cref="BusinessException">Выбрасывается, если пользователь или роль не найдены, либо при ошибке назначения.</exception>
    Task<bool> AssignRoleAsync(AssignRoleDto dto);
    /// <summary>
    /// Создание нового пользователя администратором с назначением роли.
    /// </summary>
    Task<bool> CreateUserAsync(CreateUserDto dto);

    /// <summary>
    /// Обновление основных данных пользователя (логин, email).
    /// </summary>
    Task<bool> UpdateUserAsync(UpdateUserDto dto);
}