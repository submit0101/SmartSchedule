using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Models.DTO.ReportDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с занятиями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;
   /// <summary>
   /// конструктор 
   /// </summary>
   /// <param name="lessonService">интерфейс сервиса </param>
    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Получить список всех занятий
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetAll(CancellationToken ct)
    {
        // ВНИМАНИЕ: Если данных будет > 10000, лучше добавить пагинацию в сервис.
        // Сейчас отдаем всё, как ты и просил.
        var lessons = await _lessonService.GetAllAsync(ct);
        return Ok(lessons);
    }

    /// <summary>
    /// Получить занятие по идентификатору
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseLessonDto>> GetById(int id, CancellationToken ct)
    {
        var lesson = await _lessonService.GetByIdAsync(id, ct);
        if (lesson == null) return NotFound();
        return Ok(lesson);
    }

    /// <summary>
    /// Создать новое занятие
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ResponseLessonDto>> Create(CreateLessonDto dto, CancellationToken ct)
    {
        // Валидация модели происходит автоматически благодаря [ApiController]
        try
        {
            var lesson = await _lessonService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }
        catch (ScheduleConflictException ex)
        {
            // Возвращаем красивую ошибку конфликта (409)
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Schedule Conflict",
                detail: ex.Message
            );
        }
    }

    /// <summary>
    /// Обновить занятие
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateLessonDto dto, CancellationToken ct)
    {
        try
        {
            await _lessonService.UpdateAsync(id, dto, ct);
            return NoContent();
        }
        catch (ScheduleConflictException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Schedule Conflict",
                detail: ex.Message
            );
        }
    }

    /// <summary>
    /// Удалить занятие
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _lessonService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Получить все занятия группы
    /// </summary>
    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetByGroup(int groupId, CancellationToken ct)
    {
        var lessons = await _lessonService.GetByGroupIdAsync(groupId, ct);
        return Ok(lessons);
    }

    /// <summary>
    /// Получить структурированное расписание группы
    /// </summary>
    [HttpGet("group/{groupId}/structured")]
    public async Task<ActionResult> GetStructuredSchedule(int groupId, CancellationToken ct)
    {
        // Убрали try-catch. Если упадет - Middleware сам вернет 500 безопасно.
        var schedule = await _lessonService.GetStructuredScheduleByGroupIdAsync(groupId, ct);
        return Ok(new { schedule });
    }

    /// <summary>
    /// Ищет свободные кабинеты
    /// </summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableCabinets(
        [FromQuery] int day,
        [FromQuery] int timeslot,
        [FromQuery] int weektype,
        [FromQuery] int? buildingId,
        CancellationToken ct)
    {
        var cabinets = await _lessonService.GetAvailableCabinetsAsync(day, timeslot, weektype, buildingId, ct);
        return Ok(cabinets);
    }

    /// <summary>
    /// Получить отчёт о загруженности аудиторий
    /// </summary>
    [HttpGet("cabinet-usage")]
    public async Task<IActionResult> GetCabinetUsageReport(
        [FromQuery] int? buildingId,
        [FromQuery] int? weekTypeId,
        [FromQuery] int? dayOfWeekId,
        CancellationToken ct)
    {
        var filter = new CabinetUsageFilterDto
        {
            BuildingId = buildingId,
            WeekTypeId = weekTypeId,
            DayOfWeekId = dayOfWeekId
        };

        var report = await _lessonService.GetCabinetUsageReportAsync(filter, ct);
        return Ok(report);
    }

    /// <summary>
    /// Получить расписание кабинета по ID с возможностью фильтрации по типу недели.
    /// </summary>
    [HttpGet("{cabinetId}/schedule")]
    [ProducesResponseType(typeof(List<CabinetScheduleReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCabinetScheduleAsync(int cabinetId, int weekTypeId = 0, CancellationToken ct = default)
    {
        try
        {
            var schedule = await _lessonService.GetCabinetScheduleAsync(cabinetId, weekTypeId, ct);
            return Ok(schedule);
        }
        catch (ObjectNotFoundException ex)
        {
            // Ловим 404, это нормально
            return NotFound(new { message = ex.Message });
        }
        // Общий Exception не ловим - он уйдет в безопасный Middleware
    }

    /// <summary>
    /// Предварительная проверка конфликтов (для DnD).
    /// </summary>
    [HttpGet("check-conflict")]
    [ProducesResponseType(typeof(ConflictCheckResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckConflict(
        [FromQuery] int lessonId,
        [FromQuery] int targetDayId,
        [FromQuery] int targetTimeId,
        CancellationToken ct)
    {
        var result = await _lessonService.CheckConflictAsync(lessonId, targetDayId, targetTimeId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Получение плоского расписания преподавателя
    /// </summary>
    [HttpGet("teacher/{teacherId}")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetByTeacher(int teacherId, CancellationToken ct)
    {
        var lessons = await _lessonService.GetByTeacherIdAsync(teacherId, ct);
        return Ok(lessons);
    }

    /// <summary>
    /// Получить структурированное расписание преподавателя
    /// </summary>
    [HttpGet("teacher/{teacherId}/structured")]
    public async Task<ActionResult> GetStructuredScheduleByTeacher(int teacherId, CancellationToken ct)
    {
        var schedule = await _lessonService.GetStructuredScheduleByTeacherIdAsync(teacherId, ct);
        return Ok(new { schedule });
    }

    /// <summary>
    /// Получить отчёт о загруженности преподавателей
    /// </summary>
    [HttpGet("teacher-usage")]
    [ProducesResponseType(typeof(List<TeacherUsageReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacherUsageReport(
        [FromQuery] int? weekTypeId,
        [FromQuery] int? dayOfWeekId,
        CancellationToken ct)
    {
        var filter = new TeacherUsageFilterDto
        {
            WeekTypeId = weekTypeId,
            DayOfWeekId = dayOfWeekId
        };

        var report = await _lessonService.GetTeacherUsageReportAsync(filter, ct);
        return Ok(report);
    }

    /// <summary>
    /// Получить тепловую карту расписания преподавателя
    /// </summary>
    [HttpGet("teacher/{teacherId}/schedule")]
    [ProducesResponseType(typeof(List<TeacherScheduleReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherScheduleAsync(int teacherId, [FromQuery] int weekTypeId = 0, CancellationToken ct = default)
    {
        try
        {
            var schedule = await _lessonService.GetTeacherScheduleAsync(teacherId, weekTypeId, ct);
            return Ok(schedule);
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Получает отчет о загруженности учебных групп.
    /// </summary>
    [HttpGet("group-usage")]
    [ProducesResponseType(typeof(List<GroupUsageReportDto>), 200)]
    public async Task<IActionResult> GetGroupUsageReport(
        [FromQuery] GroupUsageFilterDto filter,
        CancellationToken ct)
    {
       
        filter ??= new GroupUsageFilterDto();

        
        var report = await _lessonService.GetGroupUsageReportAsync(filter, ct);
        return Ok(report);
    }
    /// <summary>
    /// Пакетное обновление занятий (используется для одновременного переноса нескольких подгрупп).
    /// </summary>
    [HttpPut("batch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBatch([FromBody] IReadOnlyCollection<UpdateLessonDto> dtos, CancellationToken ct)
    {
        try
        {
            await _lessonService.UpdateBatchAsync(dtos, ct);
            return NoContent();
        }
        catch (ScheduleConflictException ex)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Schedule Conflict",
                detail: ex.Message
            );
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    /// <summary>
    /// Формирует динамический сводный отчет (Cross-tab) на основе выбранных параметров группировки.
    /// </summary>
    /// <param name="filter">DTO с параметрами группировки для строк и колонок отчета.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Матрица отчета с динамическими заголовками и вычисленными часами.</returns>
    /// <response code="200">Отчет успешно сгенерирован.</response>
    /// <response code="400">Ошибка в параметрах фильтрации или группировки.</response>
    /// <response code="500">Внутренняя ошибка сервера при формировании данных.</response>
    [HttpPost("crosstab")]
    [ProducesResponseType(typeof(DynamicReportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DynamicReportResultDto>> GetCrossTabReport(
        [FromBody] DynamicReportFilterDto filter,
        CancellationToken ct)
    {
        if (filter == null)
        {
            return BadRequest("Параметры фильтрации не могут быть пустыми.");
        }

        try
        {
            var result = await _lessonService.GenerateDynamicReportAsync(filter, ct).ConfigureAwait(false);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Произошла ошибка при формировании аналитического отчета.");
        }
    }
}