using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SmartSchedule.API.Validators;

/// <summary>
/// Определяет вспомогательный класс, который может использоваться для проверки объектов 
/// и вложенных объектов рекурсивно если они имеют связанные атрибуты 
/// System.ComponentModel.DataAnnotations.ValidationAttribute.
/// </summary>
public static class RecursiveValidator
{
    /// <summary>
    /// Проверяет рекурсионно, является ли данный экземпляр объекта валидным.
    /// </summary>
    /// <typeparam name="T">Тип валидируемого объекта.</typeparam>
    /// <param name="obj">Валидируемый объект.</param>
    /// <param name="results"></param>
    /// <returns><c>true</c> если объект валидный, <c>false</c>, если обнаружены какие-либо ошибки проверки.</returns>
    public static bool TryValidateObjectRecursive<T>(T obj, ICollection<ValidationResult> results) where T : class
        => TryValidateObjectRecursive(obj, results, []);

    private static bool TryValidateObjectRecursive<T>(T obj, ICollection<ValidationResult> results, HashSet<object> validatedObjects) where T : class
    {
        if (validatedObjects.Contains(obj))
            return true;

        var result = Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        validatedObjects.Add(obj);

        var properties = obj.GetType().GetProperties()
            .Where(prop => prop.CanRead && prop.GetIndexParameters().Length == 0)
            .ToArray();

        foreach (var property in properties)
        {
            if (property.PropertyType != typeof(string) && !property.PropertyType.IsValueType)
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    if (value is IEnumerable enumeration)
                    {
                        if (!TryValidateNestedEnumeration(enumeration, results, validatedObjects))
                            result = false;
                    }
                    else
                    {
                        if (!TryValidateNestedObject(value, results, validatedObjects))
                            result = false;
                    }
                }
            }
        }

        return result;
    }

    private static bool TryValidateNestedEnumeration(IEnumerable enumeration, ICollection<ValidationResult> results, HashSet<object> validatedObjects)
    {
        var result = true;

        foreach (var obj in enumeration)
        {
            if (obj != null)
            {
                if (!TryValidateNestedObject(obj, results, validatedObjects))
                    result = false;
            }
        }

        return result;
    }

    private static bool TryValidateNestedObject(object obj, ICollection<ValidationResult> results, HashSet<object> validatedObjects)
    {
        var nestedResults = new List<ValidationResult>();
        var result = TryValidateObjectRecursive(obj, nestedResults, validatedObjects);

        if (!result)
        {
            foreach (var nestedResult in nestedResults)
            {
                results.Add(nestedResult);
            }
        }

        return result;
    }
}
