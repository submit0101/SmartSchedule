using AutoMapper;
using SmartSchedule.Application.DTOs;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;

namespace SmartSchedule.Core.Service;

/// <summary>
/// Сервис для работы с днями недели
/// </summary>
public class WeekDayService : IWeekDayService
{
    #region Поля

    private readonly IWeekDayRepository _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса дней недели
    /// </summary>
    /// <param name="repository">Репозиторий дней недели</param>
    /// <param name="mapper">Сервис маппинга</param>
    public WeekDayService(IWeekDayRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить список всех дней недели в кратком формате
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список дней недели</returns>
    public async Task<List<WeekDayShortDto>> GetAllShortsAsync(CancellationToken ct)
    {
        var days = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<WeekDayShortDto>>(days);
    }

    #endregion
}
