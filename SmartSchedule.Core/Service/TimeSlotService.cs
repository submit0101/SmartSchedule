using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для работы с временными слотами
/// </summary>
public class TimeSlotService : ITimeSlotService
{
    #region Поля

    private readonly ITimeSlotRepository _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса временных слотов
    /// </summary>
    /// <param name="repository">Репозиторий временных слотов</param>
    /// <param name="mapper">Объект маппинга</param>
    /// <exception cref="ArgumentNullException">Если repository или mapper равны null</exception>
    public TimeSlotService(ITimeSlotRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить все временные слоты
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO временных слотов</returns>
    public async Task<List<ResponseTimeSlotDto>> GetAllAsync(CancellationToken ct)
    {
        var timeSlots = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseTimeSlotDto>>(timeSlots);
    }

    /// <summary>
    /// Получить временной слот по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор временного слота</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO временного слота</returns>
    public async Task<ResponseTimeSlotDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var timeSlot = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseTimeSlotDto>(timeSlot);
    }

    /// <summary>
    /// Создать новый временной слот
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO созданного временного слота</returns>
    public async Task<ResponseTimeSlotDto> CreateAsync(CreateTimeSlotDto dto, CancellationToken ct)
    {
        var timeSlot = _mapper.Map<TimeSlot>(dto);
        await _repository.CreateAsync(timeSlot, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseTimeSlotDto>(timeSlot);
    }

    /// <summary>
    /// Обновить данные временного слота
    /// </summary>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="id">Уникальный идентификатор</param>
    /// <returns>DTO обновленного временного слота</returns>
    public async Task UpdateAsync(int id, UpdateTimeSlotDto dto, CancellationToken ct)
    {
        var timeSlot = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        _mapper.Map(dto, timeSlot);
        ArgumentNullException.ThrowIfNull(timeSlot);
        await _repository.UpdateAsync(timeSlot, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удалить временной слот
    /// </summary>
    /// <param name="id">Идентификатор временного слота</param>
    /// <param name="ct">Токен отмены</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
    }

    #endregion
}