using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using EduRoomLoad.API.Errors;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace EduRoomLoad.API.Models.ExtendedProblemDetails;

/// <summary>
/// Детали ошибки валидации для RFC 7807
/// </summary>
/// <seealso cref="ProblemDetails" />
public class ValidationExceptionProblemDetails : ExtendedExceptionProblemDetails
{
    /// <summary>
    /// Создает новый экземпляр класса <see cref="ValidationExceptionProblemDetails"/>.
    /// </summary>
    /// <param name="ex">Исключение.</param>
    /// <param name="statusCode">Код ошибки.</param>
    public ValidationExceptionProblemDetails(ValidationException ex, int statusCode = StatusCodes.Status400BadRequest) : base(ex, statusCode)
    {
        ArgumentNullException.ThrowIfNull(ex);
        var errors = ex.Errors.Select(e =>
        {
            object? name = null;

            e.FormattedMessagePlaceholderValues?.TryGetValue(nameof(ValidationFailure.PropertyName), out name);
            var propertyName = name != null ? name.ToString() : e.PropertyName;

            return new
            {
                PropertyName = propertyName!,
                e.ErrorMessage
            };
        }).ToList();

        Extensions.Add("validationErrors",
            errors.Select(e => e.PropertyName).Distinct().Select(pn =>
            {
                var errorMessages = errors.Where(er => er.PropertyName == pn)
                    .Select(er => er.ErrorMessage);

                return new ValidationError(pn, errorMessages);
            }));
    }
}
