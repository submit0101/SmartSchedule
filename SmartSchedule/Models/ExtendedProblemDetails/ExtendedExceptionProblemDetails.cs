using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
namespace EduRoomLoad.API.Models.ExtendedProblemDetails;

/// <summary>Детали ошибки для RFC 7807</summary>
/// <seealso cref="ProblemDetails" />
public class ExtendedExceptionProblemDetails : ProblemDetails
{
    /// <summary>Создает новый экземпляр класса <see cref="ExtendedExceptionProblemDetails"/>.</summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="statusCode">Код ошибки.</param>
    public ExtendedExceptionProblemDetails(Exception ex, int statusCode)
    {
        Detail = ex.Message;
        Status = statusCode;
        Title = ReasonPhrases.GetReasonPhrase(statusCode);
        Type = $"https://httpstatuses.com/{statusCode}";
        Exception = ex;
    }

    /// <summary>Возвращает исключение.</summary>
    [JsonIgnore]
    public Exception Exception { get; }
}
