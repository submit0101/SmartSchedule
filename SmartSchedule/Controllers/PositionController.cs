using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.PosittonDTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с должностями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PositionController : ControllerBase
{
    #region Поля

    private readonly IPositionService _positionService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера должностей
    /// </summary>
    /// <param name="positionService">Сервис для работы с должностями</param>
    public PositionController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех должностей
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список должностей</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponsePositionDto>>> GetAll(CancellationToken ct)
    {
        var positions = await _positionService.GetAllAsync(ct);
        return Ok(positions);
    }

    /// <summary>
    /// Получить список всех должностей в коротком варианте для выпадашек
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список должностей</returns>
    [HttpGet("Short")]
    public async Task<ActionResult<List<ShortPositionDto>>> GetAllShort(CancellationToken ct)
    {
        var positions = await _positionService.GetShortAllAsync(ct);
        return Ok(positions);
    }

    /// <summary>
    /// Получить должность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные должности</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponsePositionDto>> GetById(int id, CancellationToken ct)
    {
        var position = await _positionService.GetByIdAsync(id, ct);
        return position == null ? NotFound() : Ok(position);
    }
    /// <summary>
    /// Создать новую должность.
    /// </summary>
    /// <param name="dto">Данные для создания должности.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданная должность.</returns>
    /// <response code="201">Должность успешно создана</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="409">Должность с таким названием уже существует</response>
    [Authorize(Roles = "Admin, Dispatcher")]
    [HttpPost]
    [ProducesResponseType(typeof(ResponsePositionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ResponsePositionDto>> Create(
        [FromBody] CreatePositionDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var position = await _positionService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = position.Id }, position);
        }
        catch (UniqueNameConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Конфликт уникальности",
                Detail = ex.Message,
                Extensions = { ["name"] = ex.Name },
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Внутренняя ошибка сервера",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Обновить данные должности.
    /// </summary>
    /// <param name="id">Идентификатор должности.</param>
    /// <param name="dto">Данные для обновления должности.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Результат операции.</returns>
    /// <response code="204">Должность успешно обновлена</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Должность не найдена</response>
    /// <response code="409">Должность с таким названием уже существует</response>
    [Authorize(Roles = "Admin, Dispatcher")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdatePositionDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _positionService.UpdateAsync(id, dto, ct);
            return NoContent();
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Должность не найдена",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (UniqueNameConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Конфликт уникальности",
                Detail = ex.Message,
                Extensions = { ["name"] = ex.Name },
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Внутренняя ошибка сервера",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
    /// <summary>
    /// Удалить должность
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [Authorize(Roles = "Admin, Dispatcher")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _positionService.DeleteAsync(id, ct);
        return NoContent();
    }

    #endregion
}