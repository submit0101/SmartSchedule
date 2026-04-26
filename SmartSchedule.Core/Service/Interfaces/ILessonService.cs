using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для управления занятиями и проверки конфликтов.
/// </summary>
public interface ILessonService
{
    /// <summary>
    /// Получить список всех занятий.
    /// </summary>
    Task<List<ResponseLessonDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить занятие по идентификатору.
    /// </summary>
    Task<ResponseLessonDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новое занятие.
    /// </summary>
    Task<ResponseLessonDto> CreateAsync(CreateLessonDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию о занятии.
    /// </summary>
    Task UpdateAsync(int id, UpdateLessonDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить занятие по идентификатору.
    /// </summary>
    Task DeleteAsync(int id, CancellationToken ct);

    /// <summary>
    /// Получить все занятия группы.
    /// </summary>
    Task<List<ResponseLessonDto>> GetByGroupIdAsync(int groupId, CancellationToken ct);

    /// <summary>
    /// Получить структурированное расписание группы.
    /// </summary>
    Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByGroupIdAsync(int groupId, CancellationToken ct);

    /// <summary>
    /// Ищет свободные кабинеты на указанный слот.
    /// </summary>
    Task<List<ResponseCabinetDto>> GetAvailableCabinetsAsync(int dayOfWeekId, int timeSlotId, int weekTypeId, int? buildingId, CancellationToken ct);

    /// <summary>
    /// Проверяет наличие конфликтов (Группа, Преподаватель, Кабинет).
    /// </summary>
    Task<ConflictCheckResultDto> CheckConflictAsync(int lessonId, int targetDayId, int targetTimeId, CancellationToken ct);

    /// <summary>
    /// Получить все занятия преподавателя.
    /// </summary>
    Task<List<ResponseLessonDto>> GetByTeacherIdAsync(int teacherId, CancellationToken ct);

    /// <summary>
    /// Получить структурированное расписание преподавателя.
    /// </summary>
    Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByTeacherIdAsync(int teacherId, CancellationToken ct);

    /// <summary>
    /// Пакетное обновление занятий.
    /// </summary>
    Task UpdateBatchAsync(IReadOnlyCollection<UpdateLessonDto> dtos, CancellationToken ct);
}