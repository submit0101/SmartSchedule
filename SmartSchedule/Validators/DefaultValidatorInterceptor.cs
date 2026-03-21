using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace SmartSchedule.API.Validators;

/// <summary>
/// Реализация <see cref="IValidatorInterceptor"/> по умолчанию
/// </summary>
public class DefaultValidatorInterceptor : IValidatorInterceptor
{
    /// <inheritdoc />
    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext) =>
        commonContext;

    /// <inheritdoc />
    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        => !result.IsValid ? throw new ValidationException("Validation failed", result.Errors) : result;
}
