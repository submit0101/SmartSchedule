using SmartSchedul.Core.Extensions;

namespace SmartSchedule.Core.Extensions;

/// <summary>
/// Расширения для работы с перечислениями.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Возвращает значение атрибута Description для перечисления.
    /// </summary>
    /// <typeparam name="T">Тип перечисления.</typeparam>
    /// <param name="enumType">Перечисление с атрибутами Description.</param>
    /// <returns>Значение атрибута Description.</returns>
    public static string GetDescriptionAttribute<T>(this T enumType) where T : Enum
        => enumType.GetType().GetDescriptionAttribute(enumType.ToString());
}