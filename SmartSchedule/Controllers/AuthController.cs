using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using SmartSchedule.Core.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartSchedule.Controllers;

/// <summary>
/// Контроллер для регистрации, входа и управления пользователями.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера <see cref="AuthController"/>.
    /// </summary>
    /// <param name="authService">Сервис аутентификации.</param>
    public AuthController(IAuthService authService)
    {
        ArgumentNullException.ThrowIfNull(authService);
        _authService = authService;
    }

    /// <summary>
    /// Регистрация нового пользователя. Доступно только администраторам.
    /// </summary>
    /// <param name="dto">Данные для регистрации.</param>
    /// <returns>Сообщение об успешной регистрации.</returns>
    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        await _authService.RegisterAsync(dto);
        return Ok(new { Message = "Пользователь успешно зарегистрирован." });
    }

    /// <summary>
    /// Вход в систему и получение JWT-токена.
    /// </summary>
    /// <param name="dto">Данные для входа.</param>
    /// <returns>JWT-токен и данные пользователя.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(response);
    }

    /// <summary>
    /// Получение списка всех пользователей. Доступно только администраторам.
    /// </summary>
    /// <returns>Список пользователей с их ролями.</returns>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Назначение роли пользователю. Доступно только администраторам.
    /// </summary>
    /// <param name="dto">Данные для назначения роли.</param>
    /// <returns>Сообщение об успешном назначении роли.</returns>
    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        await _authService.AssignRoleAsync(dto);
        return Ok(new { Message = "Роль успешно обновлена." });
    }
    /// <summary>
    /// Создание нового пользователя. Доступно только администраторам.
    /// </summary>
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        await _authService.CreateUserAsync(dto);
        return Ok(new { Message = "Пользователь успешно создан." });
    }

    /// <summary>
    /// Обновление данных пользователя. Доступно только администраторам.
    /// </summary>
    [HttpPut("update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
    {
        await _authService.UpdateUserAsync(dto);
        return Ok(new { Message = "Пользователь успешно обновлен." });
    }
}
