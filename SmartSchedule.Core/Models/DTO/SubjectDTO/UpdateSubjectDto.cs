using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.SubjectDTO;
/// <summary>
/// обновление с информацией о предмете.
/// </summary>
public class UpdateSubjectDto
{
    /// <summary>
    /// Уникальный идентификатор предмета.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Название предмета.
    /// </summary>
    [Required(ErrorMessage = "Название предмета обязательно")]
    [StringLength(50, ErrorMessage = "Название предмета не должно превышать 50 символов")]
    [RegularExpression(@"^[а-яА-ЯёЁ0-9\s.\-]+$",
    ErrorMessage = "Название может содержать только русские буквы, цифры, точки, дефисы и пробелы")]
    public required string Title { get; set; }
}
