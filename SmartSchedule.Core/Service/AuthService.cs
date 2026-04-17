using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using SmartSchedule.Core.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Service;

/// <summary>
/// Сервис для работы с авторизацией и управлением пользователями.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса <see cref="AuthService"/>.
    /// </summary>
    /// <param name="userManager">Менеджер пользователей Identity.</param>
    /// <param name="roleManager">Менеджер ролей Identity.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public AuthService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(roleManager);
        ArgumentNullException.ThrowIfNull(configuration);

        _userManager = userManager;
        _roleManager = roleManager;
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

    /// <inheritdoc/>
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!
            };

            foreach (var role in roles)
            {
                userDto.Roles.Add(role);
            }

            userDtos.Add(userDto);
        }

        return userDtos;
    }

    /// <inheritdoc/>
    public async Task<bool> AssignRoleAsync(AssignRoleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = await _userManager.FindByIdAsync(dto.UserId).ConfigureAwait(false);
        if (user == null)
            throw new BusinessException("Пользователь не найден.");

        if (!await _roleManager.RoleExistsAsync(dto.RoleName).ConfigureAwait(false))
            throw new BusinessException($"Роль '{dto.RoleName}' не существует в системе.");

        var currentRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        await _userManager.RemoveFromRolesAsync(user, currentRoles).ConfigureAwait(false);

        var result = await _userManager.AddToRoleAsync(user, dto.RoleName).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessException($"Ошибка при назначении роли: {errors}");
        }

        return true;
    }
    /// <inheritdoc/>
    public async Task<bool> CreateUserAsync(CreateUserDto dto)
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
            throw new BusinessException($"Ошибка при создании пользователя: {errors}");
        }

        if (!string.IsNullOrEmpty(dto.Role))
        {
            if (!await _roleManager.RoleExistsAsync(dto.Role).ConfigureAwait(false))
                throw new BusinessException($"Роль '{dto.Role}' не существует в системе.");

            await _userManager.AddToRoleAsync(user, dto.Role).ConfigureAwait(false);
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateUserAsync(UpdateUserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var user = await _userManager.FindByIdAsync(dto.UserId).ConfigureAwait(false);
        if (user == null)
            throw new BusinessException("Пользователь не найден.");

        user.UserName = dto.Username;
        user.Email = dto.Email;

        var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessException($"Ошибка при обновлении пользователя: {errors}");
        }

        return true;
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