namespace SmartSchedule.Core.Exceptions;


/// <summary>
/// Исключение, возникающее при обнаружении конфликта в расписании занятий
/// </summary>
/// <remarks>
/// Это исключение выбрасывается, когда при создании или изменении занятия 
/// обнаруживается пересечение с существующим занятием для той же группы 
/// в тот же временной слот и день недели.
/// </remarks>
public class ScheduleConflictException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ScheduleConflictException"/> 
    /// с сообщением об ошибке по умолчанию
    /// </summary>
    public ScheduleConflictException()
        : base("Ошибка: Конфликт расписания")
    { }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ScheduleConflictException"/> 
    /// с указанным сообщением об ошибке
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку</param>
    public ScheduleConflictException(string message)
        : base(message)
    { }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ScheduleConflictException"/> 
    /// с указанным сообщением об ошибке и ссылкой на внутреннее исключение
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">
    /// Исключение, которое является причиной текущего исключения
    /// </param>
    public ScheduleConflictException(string message, Exception innerException)
        : base(message, innerException)
    { }
}