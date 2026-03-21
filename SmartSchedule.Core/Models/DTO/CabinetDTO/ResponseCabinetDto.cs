#nullable disable
namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// Ответ с информацией о аудитории.
/// </summary>
public class ResponseCabinetDto
{
    /// <summary>
    /// Уникальный идентификатор аудитории.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Номер аудитории.
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// Здание или корпус, в котором находится аудитория.
    /// </summary>
    public required string BuldingName { get; set; }

    /// <summary>
    /// Вместимость аудитории (может быть null).
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Перечень оборудования, доступного в аудитории.
    /// </summary>
    public string Equipment { get; set; }
}
