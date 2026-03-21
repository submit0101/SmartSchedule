using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.API.Models.Settings;

/// <summary>
/// Модель настроек сваггера.
/// </summary>
public class SwaggerConfiguration
{
    /// <summary>Возвращает или задает флаг использования сваггера.</summary>
    /// <value>
    /// <para>Флаг использования сваггера.</para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "UseSwagger": true,
    /// </code>
    /// </example>
    [Required(ErrorMessage = $"{nameof(UseSwagger)} не найден")]
    public bool UseSwagger { get; set; }

    /// <summary>Возвращает или задает заголовок.</summary>
    /// <value>
    /// <para>Заголовок.</para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "Title": "Swagger Title",
    /// </code>
    /// </example>
    [Required(ErrorMessage = $"{nameof(Title)} не найден")]
    public required string Title { get; set; }

    /// <summary>Возвращает или задает описание.</summary>
    /// <value>
    /// <para>Описание.</para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "Description": "Swagger Description",
    /// </code>
    /// </example>
    [Required(ErrorMessage = $"{nameof(Description)} не найден")]
    public required string Description { get; set; }

    /// <summary>Фраза прокси.</summary>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "ProxyPass": "proxy",
    /// </code>
    /// </example>
    public string? ProxyPass { get; set; }
}
