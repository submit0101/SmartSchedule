using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с занятиями (CRUD и базовое расписание).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    /// <summary>
    /// Конструктор контроллера занятий.
    /// </summary>
    public LessonController(ILessonService lessonService) => _lessonService = lessonService;

    /// <summary>
    /// Получить список всех занятий.
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetAll(CancellationToken ct) => Ok(await _lessonService.GetAllAsync(ct));

    /// <summary>
    /// Получить занятие по идентификатору.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseLessonDto>> GetById(int id, CancellationToken ct)
    {
        var lesson = await _lessonService.GetByIdAsync(id, ct);
        return lesson == null ? NotFound() : Ok(lesson);
    }

    /// <summary>
    /// Создать новое занятие.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ResponseLessonDto>> Create(CreateLessonDto dto, CancellationToken ct)
    {
        try
        {
            var lesson = await _lessonService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }
        catch (ScheduleConflictException ex)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Schedule Conflict", detail: ex.Message);
        }
    }

    /// <summary>
    /// Обновить занятие.
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
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Schedule Conflict", detail: ex.Message);
        }
    }

    /// <summary>
    /// Удалить занятие.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _lessonService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Получить все занятия группы.
    /// </summary>
    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetByGroup(int groupId, CancellationToken ct) => Ok(await _lessonService.GetByGroupIdAsync(groupId, ct));

    /// <summary>
    /// Получить структурированное расписание группы.
    /// </summary>
    [HttpGet("group/{groupId}/structured")]
    public async Task<ActionResult> GetStructuredSchedule(int groupId, CancellationToken ct) => Ok(new { schedule = await _lessonService.GetStructuredScheduleByGroupIdAsync(groupId, ct) });

    /// <summary>
    /// Получить все занятия преподавателя.
    /// </summary>
    [HttpGet("teacher/{teacherId}")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetByTeacher(int teacherId, CancellationToken ct) => Ok(await _lessonService.GetByTeacherIdAsync(teacherId, ct));

    /// <summary>
    /// Получить структурированное расписание преподавателя.
    /// </summary>
    [HttpGet("teacher/{teacherId}/structured")]
    public async Task<ActionResult> GetStructuredScheduleByTeacher(int teacherId, CancellationToken ct) => Ok(new { schedule = await _lessonService.GetStructuredScheduleByTeacherIdAsync(teacherId, ct) });

    /// <summary>
    /// Ищет свободные кабинеты на слот.
    /// </summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableCabinets([FromQuery] int day, [FromQuery] int timeslot, [FromQuery] int weektype, [FromQuery] int? buildingId, CancellationToken ct) => Ok(await _lessonService.GetAvailableCabinetsAsync(day, timeslot, weektype, buildingId, ct));

    /// <summary>
    /// Предварительная проверка конфликтов (DnD).
    /// </summary>
    [HttpGet("check-conflict")]
    public async Task<IActionResult> CheckConflict([FromQuery] int lessonId, [FromQuery] int targetDayId, [FromQuery] int targetTimeId, CancellationToken ct) => Ok(await _lessonService.CheckConflictAsync(lessonId, targetDayId, targetTimeId, ct));

    /// <summary>
    /// Пакетное обновление занятий.
    /// </summary>
    [HttpPut("batch")]
    public async Task<IActionResult> UpdateBatch([FromBody] IReadOnlyCollection<UpdateLessonDto> dtos, CancellationToken ct)
    {
        try
        {
            await _lessonService.UpdateBatchAsync(dtos, ct);
            return NoContent();
        }
        catch (ScheduleConflictException ex) { return Problem(statusCode: StatusCodes.Status409Conflict, title: "Schedule Conflict", detail: ex.Message); }
        catch (ObjectNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}