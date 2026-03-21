using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.API.Models.Settings;

/// <summary>
/// Настройки приложения
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Название блока AppSettings.
    /// </summary>
    public const string AppSettingsName = "AppSettings";

    /// <summary>Возвращает или задает настройки БД.</summary>
    /// <value>
    /// <para>Настройки БД.</para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "DbOptions": {
    ///     ...
    /// },
    /// </code>
    /// Cм. описание полей <see cref="DbOptions"/>
    /// </example>
    [Required(ErrorMessage = $"Блок с настройками {nameof(DbOptions)} не найден")]
    public required DbOptions DbOptions { get; set; }

    /// <summary>Возвращает или задает настройки сваггера.</summary>
    /// <value>
    /// <para>Настройки сваггера.</para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "Swagger": {
    ///     .....
    /// },
    /// </code>
    /// См. описание полей <see cref="SwaggerConfiguration"/>
    /// </example>
    [Required(ErrorMessage = $"Блок с настройками {nameof(Swagger)} не найден")]
    public required SwaggerConfiguration Swagger { get; set; }
}
