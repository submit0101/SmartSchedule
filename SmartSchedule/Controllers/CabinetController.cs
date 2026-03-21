using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;
using SmartSchedule.Application.Services;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Exceptions;
using SmartSchedul.Core.Exceptions;
using System;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с кабинетами
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CabinetController : ControllerBase
{
    #region Поля

    private readonly ICabinetService _cabinetService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера кабинетов
    /// </summary>
    /// <param name="cabinetService">Сервис для работы с кабинетами</param>
    public CabinetController(ICabinetService cabinetService)
    {
        _cabinetService = cabinetService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех кабинетов
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список кабинетов</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseCabinetDto>>> GetAll(CancellationToken ct)
    {
        var cabinets = await _cabinetService.GetAllAsync(ct);
        return Ok(cabinets);
    }
    /// <summary>
    /// Получить список всех занятий 
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список занятий</returns>
    [HttpGet("Short")]
    public async Task<ActionResult<List<ResponseLessonDto>>> GetAllShory(CancellationToken ct)
    {
        var lessons = await _cabinetService.GetAllShortWithBuilding(ct);
        return Ok(lessons);
    }
    /// <summary>
    /// Получить кабинет по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор кабинета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные кабинета</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseCabinetDto>> GetById(int id, CancellationToken ct)
    {
        var cabinet = await _cabinetService.GetByIdAsync(id, ct);
        return cabinet == null ? NotFound() : Ok(cabinet);
    }

    /// <summary>
    /// Создать новый кабинет
    /// </summary>
    /// <param name="dto">Данные для создания кабинета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный кабинет</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseCabinetDto>> Create(CreateCabinetDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var cabinet = await _cabinetService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = cabinet.Id }, cabinet);
        }
        catch (CabinetConflictException ex)
        {
            return Conflict(new
            {
                Message = ex.Message,
                Number = ex.Number,
                BuildingId = ex.BuildingId
            });
        }
    }

    /// <summary>
    /// Обновить кабинет
    /// </summary>
    /// <param name="id">Идентификатор кабинета</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="dto">Модель</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCabinetDto dto, CancellationToken ct)
    {
        try
        {
            await _cabinetService.UpdateAsync(id, dto, ct);
            return NoContent();
        }
        catch (ObjectNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (CabinetConflictException ex)
        {
            return Conflict(new
            {
                Message = ex.Message,
                Number = ex.Number,
                BuildingId = ex.BuildingId
            });
        }
        catch (Exception )
        {
            return StatusCode(500, new { Message= "Внутренняя ошибка сервера" });
        }
    }

    /// <summary>
    /// Удалить кабинет
    /// </summary>
    /// <param name="id">Идентификатор кабинета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _cabinetService.DeleteAsync(id, ct);
        return NoContent();
    }
    /// <summary>
    /// Поиск кабинетов по номеру и/или зданию с возможностью сортировки.
    /// </summary>
    /// <param name="searchTerm">Числовой номер кабинета (например "310")</param>
    /// <param name="buildingNumber">Номер здания (1 или 2)</param>
    /// <param name="sortBy">Поле для сортировки ("number", "building")</param>
    /// <param name="descending">Направление сортировки (true - по убыванию)</param>
    /// <returns>Список кабинетов в формате DTO</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] string? searchTerm,
        [FromQuery] int? buildingNumber,
        [FromQuery] string? sortBy,
        [FromQuery] bool descending = false)
    {
        var result = await _cabinetService.SearchCabinetsAsync(searchTerm, buildingNumber, sortBy, descending);

        return Ok(result); 
    }

    #endregion
}