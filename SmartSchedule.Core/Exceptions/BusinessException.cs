namespace SmartSchedul.Core.Exceptions;

/// <summary>
/// Ошибка бизнес-логики.
/// </summary>
public class BusinessException : Exception
{
    /// <inheritdoc />
    public BusinessException()
    {
    }

    /// <inheritdoc />
    public BusinessException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
