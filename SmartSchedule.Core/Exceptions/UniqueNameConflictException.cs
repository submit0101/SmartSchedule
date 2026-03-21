using SmartSchedul.Core.Exceptions;
#nullable disable
namespace SmartSchedule.Core.Exceptions;

/// <summary>
/// Исключение, возникающее при нарушении уникальности имени
/// </summary>
[Serializable] // Добавляем поддержку сериализации
public class UniqueNameConflictException : BusinessException
{
    /// <summary>
    /// Конфликтующее имя
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UniqueNameConflictException"/>
    /// </summary>
    public UniqueNameConflictException()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UniqueNameConflictException"/> с указанным именем
    /// </summary>
    /// <param name="name">Конфликтующее имя</param>
    public UniqueNameConflictException(string name)
        : base($"Объект с именем '{name}' уже существует")
    {
        Name = name;
    }


    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="UniqueNameConflictException"/> с указанным сообщением об ошибке и внутренним исключением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public UniqueNameConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

}