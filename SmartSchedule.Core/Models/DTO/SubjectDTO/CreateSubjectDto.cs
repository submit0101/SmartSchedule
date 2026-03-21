using System.ComponentModel.DataAnnotations;

namespace SmartSchedule.Core.Models.DTO.SubjectDTO;
/// <summary>
/// DTO для создания нового предмета.
/// </summary>
public class CreateSubjectDto
{
    /// <summary>
    /// Название предмета.
    /// </summary>
    /// <example>Математический анализ</example>
    [Required(ErrorMessage = "Название предмета обязательно")]
    [StringLength(50, ErrorMessage = "Название предмета не должно превышать 50 символов")]
    [RegularExpression(@"^[а-яА-ЯёЁ0-9\s.\-]+$",
        ErrorMessage = "Название может содержать только русские буквы, цифры, точки, дефисы и пробелы")]
    public required string Title { get; set; }
}
