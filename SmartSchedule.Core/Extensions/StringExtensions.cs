using System.Globalization;

namespace SmartSchedule.Core.Extensions;

/// <summary>
/// Расширения для работы со строками.
/// </summary>
public static class StringExtensions
{
    /// <summary>Преобразует строку к snake_case.</summary>
    /// <param name="str">Строка для преобразования.</param>
    /// <returns>Строка в snake_case.</returns>
    public static string ToSnakeCase(this string str) =>
        str switch
        {
            null => throw new ArgumentNullException(nameof(str)),
            "" => throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str)),
            _ => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower(CultureInfo.CurrentCulture)
        };

    /// <summary>Преобразует строку к TitleCase.</summary>
    /// <param name="s">Строка в snake_case или camelCase для преобразования.</param>
    /// <returns>Строка в TitleCase.</returns>
    public static string ToTitleCase(this string s)
    {
        if (string.IsNullOrEmpty(s) || char.IsUpper(s[0]))
            return s;

        var stringArray = s.Split('_').Select(str =>
        {
            if (char.IsUpper(str[0]))
                return str;

            var charArray = str.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0], CultureInfo.InvariantCulture);
            return new string(charArray);
        });

        return string.Concat(stringArray);
    }
}
