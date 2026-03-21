#nullable disable
namespace SmartSchedule.Core.Exceptions;

/// <summary>
/// Исключение, возникающее при попытке создать или обновить кабинет с дублирующимися атрибутами.
/// Соответствует HTTP-статусу 409 Conflict.
/// </summary>
public class CabinetConflictException : Exception
{
    /// <summary>
    /// Номер кабинета, вызвавший конфликт.
    /// </summary>
    public string Number { get; }

    /// <summary>
    /// ID здания, в котором возник конфликт.
    /// </summary>
    public int BuildingId { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CabinetConflictException"/>.
    /// </summary>
    public CabinetConflictException()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CabinetConflictException"/> с указанным сообщением об ошибке.
    /// </summary>
    /// <param name="message">Сообщение, описывающее ошибку.</param>
    public CabinetConflictException(string message) : base(message)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CabinetConflictException"/> с указанным сообщением об ошибке
    /// и ссылкой на внутреннее исключение, вызвавшее данное исключение.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="innerException">Исключение, которое является причиной текущего исключения.</param>
    public CabinetConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CabinetConflictException"/> для конфликта дублирования кабинета.
    /// </summary>
    /// <param name="number">Номер кабинета.</param>
    /// <param name="buildingId">ID здания.</param>
    public CabinetConflictException(string number, int buildingId)
        : base($"Кабинет с номером '{number}' уже существует в здании ID {buildingId}.")
    {
        Number = number;
        BuildingId = buildingId;
    }
}