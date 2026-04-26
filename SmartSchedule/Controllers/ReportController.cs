using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.ReportDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для формирования аналитической отчетности.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    /// <summary>
    /// Конструктор контроллера отчетов.
    /// </summary>
    public ReportController(IReportService reportService) => _reportService = reportService;

    /// <summary>
    /// Получить отчёт о загруженности аудиторий.
    /// </summary>
    [HttpGet("cabinet-usage")]
    public async Task<IActionResult> GetCabinetUsageReport([FromQuery] int? buildingId, [FromQuery] int? weekTypeId, [FromQuery] int? dayOfWeekId, CancellationToken ct)
    {
        var filter = new CabinetUsageFilterDto { BuildingId = buildingId, WeekTypeId = weekTypeId, DayOfWeekId = dayOfWeekId };
        return Ok(await _reportService.GetCabinetUsageReportAsync(filter, ct));
    }

    /// <summary>
    /// Получить тепловую карту расписания кабинета.
    /// </summary>
    [HttpGet("cabinet/{cabinetId}/schedule")]
    public async Task<IActionResult> GetCabinetScheduleAsync(int cabinetId, int weekTypeId = 0, CancellationToken ct = default)
    {
        try { return Ok(await _reportService.GetCabinetScheduleAsync(cabinetId, weekTypeId, ct)); }
        catch (ObjectNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>
    /// Получить отчёт о загруженности преподавателей.
    /// </summary>
    [HttpGet("teacher-usage")]
    public async Task<IActionResult> GetTeacherUsageReport([FromQuery] int? weekTypeId, [FromQuery] int? dayOfWeekId, CancellationToken ct)
    {
        var filter = new TeacherUsageFilterDto { WeekTypeId = weekTypeId, DayOfWeekId = dayOfWeekId };
        return Ok(await _reportService.GetTeacherUsageReportAsync(filter, ct));
    }

    /// <summary>
    /// Получить тепловую карту расписания преподавателя.
    /// </summary>
    [HttpGet("teacher/{teacherId}/schedule")]
    public async Task<IActionResult> GetTeacherScheduleAsync(int teacherId, [FromQuery] int weekTypeId = 0, CancellationToken ct = default)
    {
        try { return Ok(await _reportService.GetTeacherScheduleAsync(teacherId, weekTypeId, ct)); }
        catch (ObjectNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    /// <summary>
    /// Получить отчет о загруженности учебных групп.
    /// </summary>
    [HttpGet("group-usage")]
    public async Task<IActionResult> GetGroupUsageReport([FromQuery] GroupUsageFilterDto filter, CancellationToken ct)
    {
        filter ??= new GroupUsageFilterDto();
        return Ok(await _reportService.GetGroupUsageReportAsync(filter, ct));
    }

    /// <summary>
    /// Формирует динамический сводный отчет (Cross-tab).
    /// </summary>
    [HttpPost("crosstab")]
    public async Task<ActionResult<DynamicReportResultDto>> GetCrossTabReport([FromBody] DynamicReportFilterDto filter, CancellationToken ct)
    {
        if (filter == null) return BadRequest("Параметры фильтрации не могут быть пустыми.");
        try { return Ok(await _reportService.GenerateDynamicReportAsync(filter, ct)); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// Генерирует ведомость методических окон для планирования совещаний.
    /// </summary>
    [HttpGet("methodical-windows")]
    public async Task<ActionResult<MethodicalWindowReportDto>> GetMethodicalWindowsReport([FromQuery] List<int> teacherIds, [FromQuery] int weekTypeId, CancellationToken ct)
    {
        return Ok(await _reportService.GenerateMethodicalWindowsReportAsync(teacherIds.AsReadOnly(), weekTypeId, ct));
    }
}