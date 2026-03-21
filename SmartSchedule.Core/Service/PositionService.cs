using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.PosittonDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для работы с должностями преподавателей
/// </summary>
public class PositionService : IPositionService
{
    #region Поля

    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса должностей
    /// </summary>
    /// <param name="repository">Репозиторий должностей</param>
    /// <param name="mapper">Объект маппинга</param>
    /// <exception cref="ArgumentNullException">Если repository или mapper равны null</exception>
    public PositionService(IPositionRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить все должности
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO должностей</returns>
    public async Task<List<ResponsePositionDto>> GetAllAsync(CancellationToken ct)
    {
        var positions = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponsePositionDto>>(positions);
    }

    /// <summary>
    /// Получить всех должносте  в  коротком виде
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO должностей</returns>
    public async Task<List<ShortPositionDto>> GetShortAllAsync(CancellationToken ct)
    {
        var teachers = await _repository.GetAllShorts(ct).ConfigureAwait(false);
        return _mapper.Map<List<ShortPositionDto>>(teachers);
    }

    /// <summary>
    /// Получить должность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO должности</returns>
    public async Task<ResponsePositionDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var position = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponsePositionDto>(position);
    }

    /// <summary>
    /// Создать новую должность
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO созданной должности</returns>
    public async Task<ResponsePositionDto> CreateAsync(CreatePositionDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _repository.ExistsByNameAsync(dto.Name, null, ct).ConfigureAwait(false))
            throw new UniqueNameConflictException(dto.Name);
        var position = _mapper.Map<Position>(dto);
        await _repository.CreateAsync(position, ct).ConfigureAwait(false);
        return _mapper.Map<ResponsePositionDto>(position);
    }

    /// <summary>
    /// Обновить данные должности
    /// </summary>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="id">Уникальный идентификатор</param>
    /// <returns>DTO обновленной должности</returns>
    public async Task UpdateAsync(int id, UpdatePositionDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _repository.ExistsByNameAsync(dto.Name, id, ct).ConfigureAwait(false))
            throw new UniqueNameConflictException(dto.Name);
        var position = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        _mapper.Map(dto, position);
        ArgumentNullException.ThrowIfNull(position);
        await _repository.UpdateAsync(position, ct).ConfigureAwait(false);

    }

    /// <summary>
    /// Удалить должность
    /// </summary>
    /// <param name="id">Идентификатор должности</param>
    /// <param name="ct">Токен отмены</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
    }

    #endregion
}