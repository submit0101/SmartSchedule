using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Service;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Application.Controllers;

/// <summary>
/// Контроллер для работы с преподавателями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeacherController : ControllerBase
{
    #region Поля

    private readonly ITeacherService _teacherService;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор контроллера преподавателей
    /// </summary>
    /// <param name="teacherService">Сервис для работы с преподавателями</param>
    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех преподавателей
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список преподавателей</returns>
    [HttpGet("all")]
    public async Task<ActionResult<List<ResponseTeacherDto>>> GetAll(CancellationToken ct)
    {
        var teachers = await _teacherService.GetAllAsync(ct);
        return Ok(teachers);
    }

    /// <summary>
    /// Получить список всех всех учителей в кратком варианте нужен для выподашек.
    /// </summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список зданий.</returns>
    [HttpGet("Short")]
    public async Task<ActionResult<List<ShortBuildingDto>>> GetAllShorts(CancellationToken ct)
    {
        var buildings = await _teacherService.GetShortAllAsync(ct);
        return Ok(buildings);
    }

    /// <summary>
    /// Получить преподавателя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор преподавателя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Данные преподавателя</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseTeacherDto>> GetById(int id, CancellationToken ct)
    {
        var teacher = await _teacherService.GetByIdAsync(id, ct);
        return teacher == null ? NotFound() : Ok(teacher);
    }

    /// <summary>
    /// Создать нового преподавателя
    /// </summary>
    /// <param name="dto">Данные для создания преподавателя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданный преподаватель</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseTeacherDto>> Create(CreateTeacherDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var teacher = await _teacherService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
    }

    /// <summary>
    /// Обновить преподавателя
    /// </summary>
    /// <param name="id">Идентификатор преподавателя</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTeacherDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _teacherService.UpdateAsync(id,dto, ct);
        return NoContent();
    }

    /// <summary>
    /// Удалить преподавателя
    /// </summary>
    /// <param name="id">Идентификатор преподавателя</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _teacherService.DeleteAsync(id, ct);
        return NoContent();
    }
    /// <summary>
    /// Выполняет поиск преподавателей по ФИО и фильтрацию по должности.
    /// </summary>
    /// <param name="search">Поисковая строка для поиска по ФИО.</param>
    /// <param name="positionId">Необязательный идентификатор должности для фильтрации.</param>
    /// <returns>Список преподавателей, соответствующих критериям поиска.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchTeachersAsync(
        [FromQuery] string? search,
        [FromQuery] int? positionId)
    {
        var result = await _teacherService.SearchTeachersAsync(search, positionId);

        return Ok(result);
    }
    #endregion
}