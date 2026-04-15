using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с временными слотами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TimeSlotController : ControllerBase
{
    #region Поля

    private readonly ITimeSlotService _timeSlotService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера временных слотов
    /// </summary>
    /// <param name="timeSlotService">Сервис для работы с временными слотами</param>
    public TimeSlotController(ITimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех временных слотов
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список временных слотов</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseTimeSlotDto>>> GetAll(CancellationToken ct)
    {
        var timeSlots = await _timeSlotService.GetAllAsync(ct);
        return Ok(timeSlots);
    }

    /// <summary>
    /// Получить временной слот по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор временного слота</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные временного слота</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseTimeSlotDto>> GetById(int id, CancellationToken ct)
    {
        var timeSlot = await _timeSlotService.GetByIdAsync(id, ct);
        return timeSlot == null ? NotFound() : Ok(timeSlot);
    }

    /// <summary>
    /// Создать новый временной слот
    /// </summary>
    /// <param name="dto">Данные для создания временного слота</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный временной слот</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ResponseTimeSlotDto>> Create(CreateTimeSlotDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var timeSlot = await _timeSlotService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = timeSlot.Id }, timeSlot);
    }

    /// <summary>
    /// Обновить временной слот
    /// </summary>
    /// <param name="id">Идентификатор временного слота</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTimeSlotDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _timeSlotService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Удалить временной слот
    /// </summary>
    /// <param name="id">Идентификатор временного слота</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _timeSlotService.DeleteAsync(id, ct);
        return NoContent();
    }

    #endregion
}