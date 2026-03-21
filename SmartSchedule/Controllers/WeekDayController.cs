using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.DTOs;
using SmartSchedule.Core.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SmartSchedule.API.Controllers;
/// <summary>
/// Контроллер для работы с днями недели
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeekDayController : ControllerBase
{
    #region Поля
    private readonly IWeekDayService _weekDayService;
    #endregion

    #region Конструктор
    /// <summary>
    /// Конструктор контроллера для дней недели
    /// </summary>
    /// <param name="weekDayService">Сервис для работы с днями недели</param>
    public WeekDayController(IWeekDayService weekDayService)
    {
        _weekDayService = weekDayService;
    }
    #endregion

    #region Методы
    /// <summary>
    /// Получить список всех дней недели в кратком формате
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список дней недели</returns>
    [HttpGet("short")]
    [ProducesResponseType(typeof(List<WeekDayShortDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WeekDayShortDto>>> GetAllShortsAsync(CancellationToken ct)
    {
        var result = await _weekDayService.GetAllShortsAsync(ct);
        return Ok(result);
    }
    #endregion
}
