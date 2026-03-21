using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для работы с типами недель
/// </summary>
public class WeekTypeService : IWeekTypeService
{
    #region Поля

    private readonly IWeekTypeRepository _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса типов недель
    /// </summary>
    /// <param name="repository">Репозиторий типов недель</param>
    /// <param name="mapper">Объект маппинга</param>
    /// <exception cref="ArgumentNullException">Если repository или mapper равны null</exception>
    public WeekTypeService(IWeekTypeRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить все типы недель
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO типов недель</returns>
    public async Task<List<ResponseWeekTypeDto>> GetAllAsync(CancellationToken ct)
    {
        var weekTypes = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseWeekTypeDto>>(weekTypes);
    }

    /// <summary>
    /// Получить тип недели по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO типа недели</returns>
    public async Task<ResponseWeekTypeDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var weekType = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseWeekTypeDto>(weekType);
    }

    /// <summary>
    /// Создать новый тип недели
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO созданного типа недели</returns>
    public async Task<ResponseWeekTypeDto> CreateAsync(CreateWeekTypeDto dto, CancellationToken ct)
    {
        var weekType = _mapper.Map<WeekType>(dto);
        await _repository.CreateAsync(weekType, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseWeekTypeDto>(weekType);
    }

    /// <summary>
    /// Обновить данные типа недели
    /// </summary>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="id">Уникальный идентификатор</param>
    /// <returns>DTO обновленного типа недели</returns>
    public async Task UpdateAsync(int id, UpdateWeekTypeDto dto, CancellationToken ct)
    {
        var weekType = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        _mapper.Map(dto, weekType);
        ArgumentNullException.ThrowIfNull(weekType);
        await _repository.UpdateAsync(weekType, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удалить тип недели
    /// </summary>
    /// <param name="id">Идентификатор типа недели</param>
    /// <param name="ct">Токен отмены</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
    }
    #endregion
}