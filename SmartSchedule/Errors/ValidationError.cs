using System.Collections.Generic;

namespace EduRoomLoad.API.Errors;

/// <summary>
/// Объект ошибки валидации.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Создает новый экземпляр класса <see cref="ValidationError"/>.
    /// </summary>
    /// <param name="propertyName">Наименование свойства.</param>
    /// <param name="errorMessages">Сообщения об ошибках.</param>
    public ValidationError(string propertyName, IEnumerable<string> errorMessages)
    {
        PropertyName = propertyName;
        ErrorMessages = errorMessages;
    }

    /// <summary>
    /// Возвращает или задает имя свойства с ошибкой валидации.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Возвращает или задает список с текстами ошибок.
    /// </summary>
    public IEnumerable<string> ErrorMessages { get; set; }
}
