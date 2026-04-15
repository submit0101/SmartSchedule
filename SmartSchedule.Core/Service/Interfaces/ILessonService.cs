
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с занятиями.
/// </summary>
public interface ILessonService
{
    /// <summary>
    /// Получить список всех занятий в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список занятий</returns>
    Task<List<ResponseLessonDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить занятие по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор занятия</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о занятии</returns>
    Task<ResponseLessonDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новое занятие.
    /// </summary>
    /// <param name="dto">Данные для создания занятия</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданное занятие</returns>
    Task<ResponseLessonDto> CreateAsync(CreateLessonDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию о занятии.
    /// </summary>
    /// <param name="id">Идентификатор занятия</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateLessonDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить занятие по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор занятия</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);
    /// <summary>
    /// Получить данный уроков для группы 
    /// </summary>
    /// <param name="groupId">групппа id</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<List<ResponseLessonDto>> GetByGroupIdAsync(int groupId, CancellationToken ct);
    /// <summary>
    /// получаем структурированное расписание для группы
    /// </summary>
    /// <param name="groupId">индентификатор группы</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByGroupIdAsync(int groupId, CancellationToken ct);
    /// <summary>
    /// ищет свободные кабинеты
    /// </summary>
    /// <param name="dayOfWeekId">день недели</param>
    /// <param name="timeSlotId">слот</param>
    /// <param name="weekTypeId">какая неделя</param>
    /// <param name="buildingId">Тип коруса</param>
    /// <param name="ct">токен отмны</param>
    /// <returns></returns>
    Task<List<ResponseCabinetDto>> GetAvailableCabinetsAsync(int dayOfWeekId,
    int timeSlotId,
    int weekTypeId,
    int? buildingId,
    CancellationToken ct);

    /// <summary>
    /// Формирует отчёт о загруженности аудиторий с учётом заданных фильтров.
    /// </summary>
    /// <param name="filter">Фильтр с параметрами отчёта (здание, день недели, тип недели)</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO со статистикой по аудиториям</returns>
    /// <exception cref="ArgumentNullException">Если filter равен null</exception>
    Task<List<CabinetUsageReportDto>> GetCabinetUsageReportAsync(
       CabinetUsageFilterDto filter,
       CancellationToken ct);
    /// <summary>
    /// тепловое представление кабинета
    /// </summary>
    /// <param name="cabinetId">Кабинет</param>
    /// <param name="weekTypeId">тип недели</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<List<CabinetScheduleReportDto>> GetCabinetScheduleAsync(int cabinetId, int weekTypeId, CancellationToken ct);
    /// <summary>
    /// Проверяет наличие конфликтов (Группа, Преподаватель, Кабинет) для занятия в целевом слоте.
    /// </summary>
    Task<ConflictCheckResultDto> CheckConflictAsync(int lessonId, int targetDayId, int targetTimeId, CancellationToken ct);
    /// <summary>
    /// Получить данные уроков для преподавателя
    /// </summary>
    /// <param name="teacherId">преподаватель</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<List<ResponseLessonDto>> GetByTeacherIdAsync(int teacherId, CancellationToken ct);

    /// <summary>
    /// получить структурированное расписание для преподавателя
    /// </summary>
    /// <param name="teacherId">ID преподавателя</param>
    /// <param name="ct">токен отмены</param>
    /// <returns></returns>
    Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByTeacherIdAsync(int teacherId, CancellationToken ct);

    /// <summary>
    /// Формирует отчёт о загруженности преподавателей с учётом заданных фильтров.
    /// </summary>
    /// <param name="filter">Фильтр с параметрами отчёта (день недели, тип недели)</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO со статистикой по преподавателям</returns>
    Task<List<TeacherUsageReportDto>> GetTeacherUsageReportAsync(
        TeacherUsageFilterDto filter,
        CancellationToken ct);
    /// <summary>
    /// Формирует отчёт в виде тепловой карты расписания для конкретного преподавателя.
    /// </summary>
    /// <param name="teacherId">Уникальный идентификатор преподавателя</param>
    /// <param name="weekTypeId">Тип недели (1, 2, или 0 для всех недель)</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO со статусом занятости по каждому слоту</returns>
    Task<List<TeacherScheduleReportDto>> GetTeacherScheduleAsync(int teacherId, int weekTypeId, CancellationToken ct);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <param name="filter">Фильтр для групп</param>
    /// <returns>возращает отчет по нагрузки на аудиторию</returns>
    Task<List<GroupUsageReportDto>> GetGroupUsageReportAsync(GroupUsageFilterDto filter,CancellationToken ct);
    /// <summary>
    /// Пакетное обновление занятий (например, для синхронного переноса подгрупп).
    /// </summary>
    /// <param name="dtos">Список DTO с новыми данными уроков</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateBatchAsync(IReadOnlyCollection<UpdateLessonDto> dtos, CancellationToken ct);
}