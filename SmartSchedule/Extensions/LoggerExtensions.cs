using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EduRoomLoad.API.Errors;
using System.Web;
using SmartSchedule.API.Models;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace SmartSchedule.API.Extensions;

/// <summary>
/// Расширения для логирования.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Логирует исключения.
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="context">Контекст запроса.</param>
    /// <param name="details">Детали исключения.</param>
    public static void LogExceptionProblemDetails(this ILogger logger, HttpContext context, ProblemDetails details)
    {
        ArgumentNullException.ThrowIfNull(context);
        var request = context.Request;

        var formattedRequest = new RequestDetails(request.Host.Value, request.IsHttps, request.Method, request.Path,
            request.Protocol, request.ContentType,
            request.QueryString.HasValue ? HttpUtility.UrlDecode(request.QueryString.Value) : null);

        var error = new ProblemDetailsError(formattedRequest, details);

        logger.ProcessError(JsonConvert.SerializeObject(error));
    }

    /// <summary>
    /// Логирует с уровнем "Error"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessError(this ILogger logger, string message) =>
        ProcessingErrorLevelException(logger, message, default!);

    /// <summary>
    /// Логирует с уровнем "Error"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="ex">Исключение.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessError(this ILogger logger, Exception? ex, string message) =>
        ProcessingErrorLevelException(logger, message, ex);

    private static readonly Action<ILogger, string, Exception?> ProcessingErrorLevelException =
        LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, nameof(ProcessError)), "{Item}");

    /// <summary>
    /// Логирует с уровнем "Warning"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessWarning(this ILogger logger, string message) =>
        ProcessingWarningLevelException(logger, message, default!);

    /// <summary>
    /// Логирует с уровнем "Warning"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="ex">Исключение.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessWarning(this ILogger logger, Exception? ex, string message) =>
        ProcessingWarningLevelException(logger, message, ex);

    private static readonly Action<ILogger, string, Exception?> ProcessingWarningLevelException =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1, nameof(ProcessError)), "{Item}");

    /// <summary>
    /// Логирует с уровнем "Information"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessInformation(this ILogger logger, string message) =>
        ProcessingInformationLevelException(logger, message, default!);

    /// <summary>
    /// Логирует с уровнем "Information"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="ex">Исключение.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessInformation(this ILogger logger, Exception? ex, string message) =>
        ProcessingInformationLevelException(logger, message, ex);

    private static readonly Action<ILogger, string, Exception?> ProcessingInformationLevelException =
        LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, nameof(ProcessError)), "{Item}");

    /// <summary>
    /// Логирует с уровнем "Debug"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessDebug(this ILogger logger, string message) =>
        ProcessingDebugLevelException(logger, message, default!);

    /// <summary>
    /// Логирует с уровнем "Debug"
    /// </summary>
    /// <param name="logger">Логер.</param>
    /// <param name="ex">Исключение.</param>
    /// <param name="message">Сообщение.</param>
    public static void ProcessDebug(this ILogger logger, Exception? ex, string message) =>
            ProcessingDebugLevelException(logger, message, ex);

    private static readonly Action<ILogger, string, Exception?> ProcessingDebugLevelException =
        LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, nameof(ProcessError)), "{Item}");
}
