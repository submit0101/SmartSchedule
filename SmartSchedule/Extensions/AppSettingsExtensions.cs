using SmartSchedule.API.Models.Settings;
using SmartSchedule.API.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartSchedule.API.Extensions;

/// <summary>
/// Расширение для настроек приложения.
/// </summary>
public static class AppSettingsExtensions
{
    /// <summary>
    /// Валидирует заполнение настроек приложения.
    /// </summary>
    /// <param name="appSettings"></param>
    /// <exception cref="ValidationException"></exception>
    public static void Validate(this AppSettings appSettings)
    {
        var validationResults = new List<ValidationResult>();
        if (!RecursiveValidator.TryValidateObjectRecursive(appSettings, validationResults))
        {
            var validationMessage = ValidationResultsToString(validationResults);
            throw new ValidationException(validationMessage);
        }
    }

    private static string ValidationResultsToString(IEnumerable<ValidationResult> validationResults)
    {
        var stringBuilder = new StringBuilder("Ошибки заполнения AppSettings: ");

        foreach (var validationResult in validationResults)
        {
            stringBuilder.Append(validationResult.ToString());
            stringBuilder.Append("; ");
        }

        return stringBuilder.ToString();
    }
}
