using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System;
using SmartSchedule.Core.Exceptions;
using SmartSchedul.Core.Exceptions;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с группами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    #region Поля

    private readonly IGroupService _groupService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера групп
    /// </summary>
    /// <param name="groupService">Сервис для работы с группами</param>
    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех групп
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список групп</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseGroupDto>>> GetAll(CancellationToken ct)
    {
        var groups = await _groupService.GetAllAsync(ct);
        return Ok(groups);
    }

    /// <summary>
    /// Получить группу по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные группы</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseGroupDto>> GetById(int id, CancellationToken ct)
    {
        var group = await _groupService.GetByIdAsync(id, ct);
        return group == null ? NotFound() : Ok(group);
    }

    /// <summary>
    /// Создать новую группу
    /// </summary>
    /// <param name="dto">Данные для создания группы</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданная группа</returns>
    /// <response code="201">Группа успешно создана</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="409">Группа с таким именем уже существует</response>
    [HttpPost]
    [ProducesResponseType(typeof(ResponseGroupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ResponseGroupDto>> Create(
        [FromBody] CreateGroupDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var group = await _groupService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }
        catch (UniqueNameConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Конфликт имени группы",
                Detail = ex.Message,
                Extensions = { ["groupName"] = ex.Name },
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
    /// Обновить группу
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    /// <response code="204">Группа успешно обновлена</response>
    /// <response code="400">Некорректные данные</response>
    /// <response code="404">Группа не найдена</response>
    /// <response code="409">Группа с таким именем уже существует</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateGroupDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _groupService.UpdateAsync(id, dto, ct);
            return NoContent();
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Группа не найдена",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (UniqueNameConflictException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Конфликт имени группы",
                Detail = ex.Message,
                Extensions = { ["groupName"] = ex.Name },
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (Exception )
        {

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Внутренняя ошибка сервера",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }


    /// <summary>
    /// Удалить группу
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _groupService.DeleteAsync(id, ct);
        return NoContent();
    }

    #endregion
}