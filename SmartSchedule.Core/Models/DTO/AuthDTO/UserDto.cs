namespace SmartSchedule.Core.Models.DTO.AuthDTO;

/// <summary>
/// DTO для отображения информации о пользователе в списке администратора.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Список ролей, назначенных пользователю.
    /// </summary>
    public IList<string> Roles { get; init; } = new List<string>();
}