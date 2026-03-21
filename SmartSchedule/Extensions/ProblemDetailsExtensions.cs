using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EduRoomLoad.API.Models.ExtendedProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;
using SmartSchedule.API.Extensions;
using SmartSchedul.Core.Exceptions;
using EduRoomLoad.API;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EduRoomLoad.Extensions;

/// <summary>
/// Расширения идентификации исключений.
/// </summary>
public static class ProblemDetailsExtensions
{
    /// <summary>
    /// Конфигурирует ответы сервера по RFC 7807.
    /// </summary>
    /// <param name="options">Настройки конфигурации.</param>
    public static void ConfigureProblemDetails(ProblemDetailsOptions options)
    {

        options.ShouldLogUnhandledException = (_, _, _) => false;
        options.OnBeforeWriteDetails = (ctx, details) =>
        {
            var logger = ctx.RequestServices.GetRequiredService<ILogger<Startup>>();

            logger.LogExceptionProblemDetails(ctx, details);

            if (details is ExtendedExceptionProblemDetails)
                return;

            var statuses = new List<int> { StatusCodes.Status401Unauthorized, StatusCodes.Status403Forbidden };
            if (statuses.Contains(details.Status ?? 0))
                details.Detail = details.Title;
        };

        options.Map<ValidationException>(ex => new ValidationExceptionProblemDetails(ex));
        options.Map<BusinessException>(ex => new ExtendedExceptionProblemDetails(ex, StatusCodes.Status400BadRequest));
        options.Map<ObjectNotFoundException>(ex => new ExtendedExceptionProblemDetails(ex, StatusCodes.Status404NotFound));
        options.Map<DbUpdateConcurrencyException>(ex => new ExtendedExceptionProblemDetails(ex, StatusCodes.Status409Conflict));
        options.Map<Exception>(ex => new ExtendedExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
    }
}
