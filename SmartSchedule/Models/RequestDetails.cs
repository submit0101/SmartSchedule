namespace SmartSchedule.API.Models;

/// <summary>
/// Моедль запроса к серверу.
/// </summary>
public class RequestDetails
{
    /// <summary>
    /// Создает новый экземпляр класса <see cref="RequestDetails"/>.
    /// </summary>
    /// <param name="contentType">Тип контента.</param>
    /// <param name="host">Хост.</param>
    /// <param name="isHttps">Флаг использования https протокола.</param>
    /// <param name="method">Метод запроса.</param>
    /// <param name="path">Путь.</param>
    /// <param name="protocol">Протокол.</param>
    /// <param name="queryString">Параметры запроса.</param>
    public RequestDetails(string host, bool isHttps, string method, string path, string protocol, string? contentType = null, string? queryString = null)
    {
        ContentType = contentType;
        Host = host;
        IsHttps = isHttps;
        Method = method;
        Path = path;
        Protocol = protocol;
        QueryString = queryString;
    }

    /// <summary>
    /// Возвращает или задает тип контента.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Возвращает или задает хост запроса.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Возвращает или задает флаг Https запроса.
    /// </summary>
    public bool IsHttps { get; set; }

    /// <summary>
    /// Возвращает или задает метод запроса.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Возвращает или задает путь запроса.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Возвращает или задает протокол запроса.
    /// </summary>
    public string Protocol { get; set; }

    /// <summary>
    /// Возвращает или задает параметры запроса.
    /// </summary>
    public string? QueryString { get; set; }
}
