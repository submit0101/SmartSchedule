using SmartSchedule.Application.DTOs;

namespace SmartSchedule.Core.Service.Interfaces;
/// <summary>
/// Интрфейс сервиса для реализации пока дня недели 
/// </summary>
public interface IWeekDayService
{
    /// <summary>
    /// Получить список всех дней в кратком формате
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список чеков</returns>
    Task<List<WeekDayShortDto>> GetAllShortsAsync(CancellationToken ct);
}