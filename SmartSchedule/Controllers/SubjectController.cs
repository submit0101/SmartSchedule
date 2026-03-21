using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Models.DTO.SubjectDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Core.Exceptions;
using System;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с предметами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SubjectController : ControllerBase
{
    #region Поля

    private readonly ISubjectService _subjectService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера предметов
    /// </summary>
    /// <param name="subjectService">Сервис для работы с предметами</param>
    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех предметов
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список предметов</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseSubjectDto>>> GetAll(CancellationToken ct)
    {
        var subjects = await _subjectService.GetAllAsync(ct);
        return Ok(subjects);
    }

    /// <summary>
    /// Получить предмет по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные предмета</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseSubjectDto>> GetById(int id, CancellationToken ct)
    {
        var subject = await _subjectService.GetByIdAsync(id, ct);
        return subject == null ? NotFound() : Ok(subject);
    }

    /// <summary>
    /// Создать новый предмет
    /// </summary>
    /// <param name="dto">Данные для создания предмета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный предмет</returns>
    /// <response code="201">Предмет успешно создан</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="409">Предмет с таким названием уже существует</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseSubjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ResponseSubjectDto>> Create(
        [FromBody] CreateSubjectDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var subject = await _subjectService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
        }
        catch (UniqueNameConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Конфликт имени предмета",
                Detail = ex.Message,
                Extensions = { ["subjectName"] = ex.Name },
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
    /// Обновить предмет
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Предмет успешно обновлен</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Предмет не найден</response>
    /// <response code="409">Предмет с таким названием уже существует</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateSubjectDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _subjectService.UpdateAsync(id, dto, ct);
            return NoContent();
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Предмет не найден",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (UniqueNameConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Конфликт имени предмета",
                Detail = ex.Message,
                Extensions = { ["subjectName"] = ex.Name },
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
    /// Удалить предмет
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _subjectService.DeleteAsync(id, ct);
        return NoContent();
    }

    #endregion
}