using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с типами недель
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeekTypeController : ControllerBase
{
    #region Поля

    private readonly IWeekTypeService _weekTypeService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера типов недель
    /// </summary>
    /// <param name="weekTypeService">Сервис для работы с типами недель</param>
    public WeekTypeController(IWeekTypeService weekTypeService)
    {
        _weekTypeService = weekTypeService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех типов недель
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список типов недель</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseWeekTypeDto>>> GetAll(CancellationToken ct)
    {
        var weekTypes = await _weekTypeService.GetAllAsync(ct);
        return Ok(weekTypes);
    }

    /// <summary>
    /// Получить тип недели по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные типа недели</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseWeekTypeDto>> GetById(int id, CancellationToken ct)
    {
        var weekType = await _weekTypeService.GetByIdAsync(id, ct);
        return weekType == null ? NotFound() : Ok(weekType);
    }

    /// <summary>
    /// Создать новый тип недели
    /// </summary>
    /// <param name="dto">Данные для создания типа недели</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный тип недели</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseWeekTypeDto>> Create(CreateWeekTypeDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var weekType = await _weekTypeService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = weekType.Id }, weekType);
    }

    /// <summary>
    /// Обновить тип недели
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateWeekTypeDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _weekTypeService.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Удалить тип недели
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _weekTypeService.DeleteAsync(id, ct);
        return NoContent();
    }

    #endregion
}