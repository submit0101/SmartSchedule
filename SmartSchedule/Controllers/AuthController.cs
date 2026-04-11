using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using SmartSchedule.Core.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartSchedule.Controllers;

/// <summary>
/// Контроллер для регистрации и входа пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    /// <summary>
    /// Конструктор
    /// </summary>
    public AuthController(IAuthService authService)
    {
        ArgumentNullException.ThrowIfNull(authService);
        _authService = authService;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        await _authService.RegisterAsync(dto);
        return Ok(new { Message = "Пользователь успешно зарегистрирован." });
    }

    /// <summary>
    /// Вход в систему и получение JWT-токена.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return Ok(response); 
    }
}