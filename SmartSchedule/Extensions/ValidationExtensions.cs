using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SmartSchedule.API.Validators;
using System.Collections.Generic;
using System.Linq;

namespace EduRoomLoad.API.Extensions;

/// <summary>
/// Расширения валидатора.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Конфигурирует исключение валидации.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <exception cref="ValidationException">Исключение валидации.</exception>
    public static void ConfigureValidationException(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = actionContext =>
        {
            
            var errors = actionContext.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            
            return new BadRequestObjectResult(new
            {
                title = "Ошибка валидации",
                status = 400,
                errors = errors
            });
        });

        services.AddTransient<IValidatorInterceptor, DefaultValidatorInterceptor>();
    }
}
