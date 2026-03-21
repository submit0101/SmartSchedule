using System;
namespace SmartSchedule.Core.Constants;
/// <summary>
/// Содержит константы для известных ID типов недели в базе данных WeekType.
/// </summary>
public static class WeekTypeConstants
{
    /// <summary>
    /// Идентификатор для нечётной недели (обычно 1).
    /// </summary>
    public const int Odd = 1;

    /// <summary>
    /// Идентификатор для чётной недели (обычно 2).
    /// </summary>
    public const int Even = 2;

    /// <summary>
    /// Идентификатор для обозначения обеих недель (обычно 3). Используется для общих занятий
    /// или для поиска конфликтов расписания.
    /// </summary>
    public const int Both = 3;
}