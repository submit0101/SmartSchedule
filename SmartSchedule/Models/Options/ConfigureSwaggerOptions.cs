using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SmartSchedule.API.Models.Settings;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace EduRoomLoad.API.Models.Options;

/// <summary>
/// Класс конфигурации сваггера.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    #region Поля
    /// <summary>
    /// Поставщик заголовков версий API.
    /// </summary>
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    /// Настройки сваггера.
    /// </summary>
    private readonly SwaggerConfiguration _configuration;
    #endregion

    #region Конструкторы
    /// <summary>
    /// Создает новый экземпляр класса <see cref="ConfigureSwaggerOptions"/>.
    /// </summary>
    /// <param name="provider">Поставщик заголовков версий API.</param>
    /// <param name="serviceProvider">Поставщик сервисов.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IServiceProvider serviceProvider)
    {
        _provider = provider;

        using var scope = serviceProvider.CreateScope();
        _configuration = scope.ServiceProvider.GetRequiredService<SwaggerConfiguration>();
    }
    #endregion

    #region Методы
    /// <summary>
    /// Конфигурирует сваггер.
    /// </summary>
    /// <param name="options">Опции сваггера.</param>
    public void Configure(SwaggerGenOptions options) =>
        Configure(options, _configuration);

    /// <summary>
    /// Конфигурирует сваггер.
    /// </summary>
    /// <param name="options">Опции сваггера.</param>
    /// <param name="configuration">Настройки сваггера.</param>
    public void Configure(SwaggerGenOptions options, SwaggerConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(configuration);

        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description, configuration));
        }
    }

    /// <summary>
    /// Создает объект с информацией для конфигурации сваггера.
    /// </summary>
    /// <param name="description">Заголовок версии API.</param>
    /// <param name="configuration">Настройки сваггера.</param>
    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, SwaggerConfiguration configuration)
    {
        var info = new OpenApiInfo
        {
            Title = $"{configuration.Title} {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
            Description = configuration.Description
        };

        if (description.IsDeprecated)
            info.Description += "Эта версия API устарела.";

        return info;
    }
    #endregion
}
