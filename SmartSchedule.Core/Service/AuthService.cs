using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using SmartSchedule.Core.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartSchedule.Core.Service;

/// <summary>
/// Сервис для работы с авторизацией
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса <see cref="AuthService"/>.
    /// </summary>
    /// <param name="userManager">Менеджер пользователей Identity.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        // Используем современный способ проверки на null
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(configuration);

        _userManager = userManager;
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var userExists = await _userManager.FindByNameAsync(dto.Username).ConfigureAwait(false);
        if (userExists != null)
            throw new BusinessException("Пользователь с таким логином уже существует.");

        IdentityUser user = new()
        {
            UserName = dto.Username,
            Email = dto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, dto.Password).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessException($"Ошибка при регистрации: {errors}");
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = await _userManager.FindByNameAsync(dto.Username).ConfigureAwait(false);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password).ConfigureAwait(false))
            throw new BusinessException("Неверный логин или пароль.");

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GetToken(authClaims);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
            Username = user.UserName!,
            Role = userRoles.FirstOrDefault()
        };
    }

    /// <summary>
    /// Вспомогательный метод для генерации JWT токена.
    /// </summary>
    /// <param name="authClaims">Список утверждений о пользователе.</param>
    /// <returns>Экземпляр <see cref="JwtSecurityToken"/>.</returns>
    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var secret = _configuration["JwtSettings:Secret"];

        
        ArgumentNullException.ThrowIfNull(secret, "JWT Secret key is not configured in appsettings.");

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}