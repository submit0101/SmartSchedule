using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EduRoomLoad.API.Extensions;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SmartSchedule.API.Extensions;
using SmartSchedule.API.Models.Settings;
using SmartSchedule.Core.Service;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Extensions;
using SmartSchedule.Infrastructure.Caching;
using SmartSchedule.Infrastructure.Data;
using StackExchange.Redis;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.RateLimiting;
using ProblemDetailsExtensions = EduRoomLoad.Extensions.ProblemDetailsExtensions;

namespace EduRoomLoad.API;

/// <summary>
/// Класс Startup для настройки конфигурации API сервисов и конвейера.
/// </summary>
internal class Startup
{
    private readonly AppSettings _appSettings;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        HostingEnvironment = env;

        _appSettings = Configuration.GetSection(AppSettings.AppSettingsName).Get<AppSettings>()
            ?? throw new ValidationException("Блок с настройками AppSettings не найден");

        _appSettings.Validate();
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment HostingEnvironment { get; }

    /// <summary>
    /// Регистрация системных и пользовательских сервисов.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddProblemDetails(ProblemDetailsExtensions.ConfigureProblemDetails);

        services.AddCors(options => options.AddPolicy("AllowWebClient", policy =>
            policy.WithOrigins("https://localhost:7294", "http://192.168.1.114:5000") // Адрес твоего Web-сайта
                  .AllowAnyHeader()
                  .AllowAnyMethod()));

        services.AddRepositories();
        services.AddServices();

        services.AddScoped<IAuthService, AuthService>();

        // Настройка БД
        if (_appSettings.DbOptions.UseInMemory == true)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("page_template"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_appSettings.DbOptions.ConnectionString));
        }

        // Настройка Identity для API (Проверка паролей)
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Настройка JWT (Для Android и Web-сайта)
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? string.Empty))
            };
        });

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
            ConnectionMultiplexer.Connect(Configuration["Redis"] ?? string.Empty));

        services.AddSingleton<ICachingService, RedisCachingService>();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(SmartSchedule.Core.Mapper.BuildingProfile).Assembly));
    }

    /// <summary>
    /// Настройка конвейера обработки HTTP-запросов.
    /// </summary>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, AppDbContext dbContext)
    {
        if (_appSettings.DbOptions.UseInMemory == true)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }
        else
        {
            dbContext.Database.Migrate();
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors("AllowWebClient"); // Используем новое имя политики
        app.UseRateLimiter();
        app.UseProblemDetails();
        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        if (_appSettings.Swagger?.UseSwagger == true)
        {
            app.UseSwagger(env, provider, _appSettings.Swagger);
        }
    }
}