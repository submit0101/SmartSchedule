using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EduRoomLoad.API.Extensions;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SmartSchedule.API.Extensions;
using SmartSchedule.API.Models.Settings;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Extensions;
using SmartSchedule.Infrastructure.Caching;
using SmartSchedule.Infrastructure.Data;
using StackExchange.Redis;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.RateLimiting;
using ProblemDetailsExtensions = EduRoomLoad.Extensions.ProblemDetailsExtensions;

namespace EduRoomLoad.API;

/// <summary>
/// Класс-конфигуратор
/// </summary>
internal class Startup
{
    #region Поля
    /// <summary>
    /// Настройки приложения.
    /// </summary>
    private readonly AppSettings _appSettings;
    #endregion

    #region Конструкторы
    /// <summary>
    /// Создает новый экземпляр класса <see cref="Startup"/>.
    /// </summary>
    /// <param name="configuration">Конфигурация сервиса.</param>
    /// <param name="env">Окружение хостинга.</param>
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        HostingEnvironment = env;

        _appSettings = Configuration.GetSection(AppSettings.AppSettingsName).Get<AppSettings>()
            ?? throw new ValidationException("Блок с настройками AppSettings не найден");

        _appSettings.Validate();
    }
    #endregion

    #region Свойства

    /// <summary>
    /// Конфигурация сервиса.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Окружение хостинга.
    /// </summary>
    public IWebHostEnvironment HostingEnvironment { get; }
    #endregion

    #region Методы
    /// <summary>
    /// Конфигурирует сервисы.
    /// </summary>
    /// <param name="services">Список сервисов.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddProblemDetails(ProblemDetailsExtensions.ConfigureProblemDetails);

        services.AddCors(options => options.AddPolicy("AllowRazorPages", policy =>
            policy.WithOrigins("https://localhost:7294", "http://192.168.1.114:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()));

        services.AddRepositories();
        services.AddServices();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 30,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0,
                    });
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Status = 429,
                    Title = "Too Many Requests",
                    Detail = "Слишком много запросов. Подождите 10 секунд."
                }, token);
            };
        });

        services.AddControllers()
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            });

        services.AddFluentValidationAutoValidation();

        services.ConfigureValidationException();

        services.AddApiVersioning(o =>
        {
            o.ReportApiVersions = true;
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        if (_appSettings.Swagger is { UseSwagger: true })
            services.AddSwagger(_appSettings.Swagger);

        services.AddHttpContextAccessor();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(Configuration["Redis"]));

        services.AddSingleton<ICachingService, RedisCachingService>();

        if (_appSettings.DbOptions.UseInMemory.HasValue && _appSettings.DbOptions.UseInMemory.Value)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("page_template"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_appSettings.DbOptions.ConnectionString));
        }

        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(SmartSchedule.Core.Mapper.BuildingProfile).Assembly));
    }

    /// <summary>
    /// Конфигурирует сервис.
    /// </summary>
    /// <param name="app">Построитель сервиса.</param>
    /// <param name="env">Окружение сервиса.</param>
    /// <param name="provider">Поставщик заголовков версий API.</param>
    /// <param name="dbContext">Контекст базы данных.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, AppDbContext dbContext)
    {
        dbContext.Database.Migrate();

        if (_appSettings.DbOptions.UseInMemory == true)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCors("AllowRazorPages");

        app.UseRateLimiter();

        app.UseProblemDetails();
        app.UseRequestLocalization();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        if (_appSettings.Swagger?.UseSwagger == true)
        {
            app.UseSwagger(env, provider, _appSettings.Swagger);
        }
    }

    #endregion
}