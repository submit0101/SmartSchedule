#nullable disable
namespace SmartSchedule.Core.Models.DTO.CabinetDTO;
/// <summary>
/// краткая модель ответа 
/// </summary>
public class ShortCabinetDto
{
    /// <summary>
    /// Уникальный Идентифатор поля нужен для серверного ответа и отправки
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Номер кабинета ну или cпз
    /// </summary>
    public string number { get; set; }
    /// <summary>
    /// Название  корпуса 
    /// </summary>
    public string BuildingName { get; set; }

}
