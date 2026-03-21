namespace SmartSchedul.Core.Exceptions;

/// <summary>
/// Исключение - объект не найден.
/// </summary>
public class ObjectNotFoundException : Exception
{
    /// <inheritdoc />
    public ObjectNotFoundException()
    {
    }

    /// <inheritdoc />
    public ObjectNotFoundException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
