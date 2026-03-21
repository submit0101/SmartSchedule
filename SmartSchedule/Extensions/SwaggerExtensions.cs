using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using EduRoomLoad.API.Models.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using SmartSchedule.API.Models.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System;

namespace EduRoomLoad.API.Extensions;

/// <summary>
/// Расширения сваггера.
/// </summary>
public static class SwaggerExtensions
{
    private static readonly string[] setupAction =
            [
                "SmartSchedule.xml",
                "SmartSchedule.Core.xml",
                "SmartSchedule.Infrastructur.xml"
            ];

    /// <summary>
    /// Конфигурирует сваггер.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Настройки сваггера.</param>
    public static void AddSwagger(this IServiceCollection services, SwaggerConfiguration configuration)
    {
        services.AddScoped(_ => configuration);
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {

            var xmlFilesPaths = setupAction;

            foreach (var xmlFilePath in xmlFilesPaths)
            {
                var path = Path.Combine(AppContext.BaseDirectory, xmlFilePath);
                options.IncludeXmlComments(path);
            }
        });

        services.AddSwaggerGenNewtonsoftSupport();
    }

    /// <summary>
    /// Добавляет ПО промежуточного слоя сваггера.
    /// </summary>
    /// <param name="app">Построитель сервиса.</param>
    /// <param name="env">Окружение сервиса.</param>
    /// <param name="provider">Поставщик заголовков версий API.</param>
    /// <param name="configuration">Настройки сваггера.</param>
    public static void UseSwagger(this IApplicationBuilder app,
        IWebHostEnvironment env,
        IApiVersionDescriptionProvider provider,
        SwaggerConfiguration configuration)
    {
        app.UseSwagger(so =>
        {
            if (!env.IsDevelopment() && !string.IsNullOrEmpty(configuration.ProxyPass))
            {
                so.PreSerializeFilters.Add((swaggerDoc, _) => swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new() {Url = configuration.ProxyPass}
                });
            }
        });
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });
    }
}
