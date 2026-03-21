using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.GroupDTO;
/// <summary>
/// DTO для обновления группы.
/// </summary>
public class UpdateGroupDto
{
    /// <summary>
    /// Идентификатор группы, которую нужно обновить.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Новое название группы (если требуется изменить).
    /// </summary>
    /// <example>22-ПРО-2</example>
    [Required(ErrorMessage = "Название группы обязательно")]
    [StringLength(10, ErrorMessage = "Название группы не должно превышать 10 символов")]
    [RegularExpression(@"^[1-9]\d?-[A-ZА-Я]{3}-[1-4]$",
    ErrorMessage = "Название группы должно быть в формате: 1-99тире-трисимвола-1-4 (пример: 99-ABC-1)")]
    public required string Name { get; set; }
}
