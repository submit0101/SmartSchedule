using System.ComponentModel;
using System.Reflection;

namespace SmartSchedul.Core.Extensions;

/// <summary>
/// Расширение для работы с типами.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Возвращает значение атрибута Description для поля по имени.
    /// </summary>
    /// <param name="type">Тип с атрибутами Description.</param>
    /// <param name="fieldName">Имя поля с атрибутом Description.</param>
    /// <returns>Значение атрибута Description.</returns>
    public static string GetDescriptionAttribute(this Type type, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(type);
        var field = type.GetField(fieldName);

        if (field == null)
            return fieldName;

        var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>(false);

        return descriptionAttribute?.Description ?? fieldName;
    }

    /// <summary>
    /// Возвращает значение атрибута DefaultValue для поля по имени.
    /// </summary>
    /// <param name="type">Тип с атрибутами DefaultValue.</param>
    /// <param name="propertyName">Имя свойства с атрибутом DefaultValue.</param>
    /// <returns>Значение атрибута DefaultValue.</returns>
    public static string GetDefaultValueAttribute(this Type type, string propertyName)
    {
        var pdc = TypeDescriptor.GetProperties(type)[propertyName]
            ?? throw new ArgumentException($"{type} don't have field with name {propertyName}", nameof(type));

        var defaultValueAttribute = pdc.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute
            ?? throw new ArgumentException($"{propertyName} don't have DefaultValueAttribute", nameof(propertyName));

        var value = defaultValueAttribute.Value?.ToString()
            ?? throw new ArgumentException($"DefaultValueAttribute into {propertyName} don't have value", nameof(propertyName));

        return value;
    }
}
