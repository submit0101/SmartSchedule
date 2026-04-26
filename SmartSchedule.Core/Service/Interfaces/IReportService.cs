using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.ReportDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Service.Interfaces;
/// <summary>
/// Интерфейс сервиса для формирования аналитических отчетов и ведомостей.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Формирует отчёт о загруженности преподавателей.
    /// </summary>
    /// <param name="filter">Фильтр (тип недели, день).</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список данных о загруженности преподавателей.</returns>
    Task<List<TeacherUsageReportDto>> GetTeacherUsageReportAsync(TeacherUsageFilterDto filter, CancellationToken ct);

    /// <summary>
    /// Получает тепловую карту расписания преподавателя.
    /// </summary>
    /// <param name="teacherId">ID преподавателя.</param>
    /// <param name="weekTypeId">Тип недели.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список слотов со статусом занятости.</returns>
    Task<List<TeacherScheduleReportDto>> GetTeacherScheduleAsync(int teacherId, int weekTypeId, CancellationToken ct);

    /// <summary>
    /// Получает отчет о загруженности учебных групп.
    /// </summary>
    /// <param name="filter">Фильтр для групп.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список данных о нагрузке на группы.</returns>
    Task<List<GroupUsageReportDto>> GetGroupUsageReportAsync(GroupUsageFilterDto filter, CancellationToken ct);

    /// <summary>
    /// Формирует отчёт о загруженности аудиторий.
    /// </summary>
    /// <param name="filter">Фильтр (здание, день, тип недели).</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Статистика по аудиториям.</returns>
    Task<List<CabinetUsageReportDto>> GetCabinetUsageReportAsync(CabinetUsageFilterDto filter, CancellationToken ct);

    /// <summary>
    /// Получает расписание занятий кабинета для визуализации.
    /// </summary>
    /// <param name="cabinetId">ID кабинета.</param>
    /// <param name="weekTypeId">Тип недели.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список слотов со статусом занятости кабинета.</returns>
    Task<List<CabinetScheduleReportDto>> GetCabinetScheduleAsync(int cabinetId, int weekTypeId, CancellationToken ct);

    /// <summary>
    /// Генерирует динамический сводный отчет (Cross-tab).
    /// </summary>
    /// <param name="filter">Параметры группировки строк и колонок.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Матрица отчета с вычисленными часами.</returns>
    Task<DynamicReportResultDto> GenerateDynamicReportAsync(DynamicReportFilterDto filter, CancellationToken ct = default);

    /// <summary>
    /// Генерирует ведомость методических окон для группы преподавателей.
    /// </summary>
    /// <param name="teacherIds">Список ID преподавателей.</param>
    /// <param name="weekTypeId">Тип недели.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Бланк отчета с найденными окнами.</returns>
    Task<MethodicalWindowReportDto> GenerateMethodicalWindowsReportAsync(ReadOnlyCollection<int> teacherIds, int weekTypeId, CancellationToken ct = default);
}
