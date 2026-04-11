using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.AuthDTO;
/// <summary>
/// DTO ответа при успешной авторизации или регистрации.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT токен для доступа к защищенным эндпоинтам API.
    /// Передается в заголовке Authorization: Bearer {Token}.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Уникальный идентификатор пользователя в базе данных (Identity UserId).
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Имя авторизованного пользователя.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Основная роль пользователя в системе (например: Admin, Teacher, Student).
    /// Используется клиентом для скрытия или показа элементов UI.
    /// </summary>
    public string? Role { get; set; }
}