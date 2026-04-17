using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Models.DTO.AuthDTO;
/// <summary>
/// Принимать запрос на смену роли
/// </summary>
public class AssignRoleDto
{
    /// <summary>
    /// Идентификатор Пользователя
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    /// <summary>
    /// Имя роли
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}
