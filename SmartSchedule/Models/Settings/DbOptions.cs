using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.API.Models.Settings;

/// <summary>
/// Модель настроек бд.
/// </summary>
public  class DbOptions
{
    /// <summary>Возвращает или задает строку подключения к БД по умолчанию.</summary>
    /// <value>
    /// <para>Строка подключения к базе данных.</para>
    /// <para>Документация PostgreSQL connection strings: https://www.connectionstrings.com/postgresql/ </para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "ConnectionString": "User ID=userId;Password=password;Host=host;Port=port;Database=dbName;Pooling=true;",
    /// </code>
    /// </example>
    [Required(ErrorMessage = $"{nameof(ConnectionString)} не найден")]
    public required string ConnectionString { get; set; }

    /// <summary>Возвращает или задает флаг использования бд в памяти.</summary>
    /// <value>
    /// <para>Флаг использования бд в памяти.</para>
    /// <para>Используется при разработке, в продуктовой версии указывать значение false или не указывать вообще.</para>
    /// </value>
    /// <example>
    /// В файле appsettings.json :
    /// <code>
    /// "UseInMemory": true,
    /// </code>
    /// </example>
    public bool? UseInMemory { get; set; }
}
