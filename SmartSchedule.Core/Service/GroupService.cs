using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Core.Service;

/// <summary>
/// Сервис для работы с группами
/// </summary>
public class GroupService : IGroupService
{
    #region Поля

    private readonly IGroupRepository _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса групп
    /// </summary>
    /// <param name="repository">Репозиторий групп</param>
    /// <param name="mapper">Объект маппинга</param>
    /// <exception cref="ArgumentNullException">Если repository или mapper равны null</exception>
    public GroupService(IGroupRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить все группы
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO групп</returns>
    public async Task<List<ResponseGroupDto>> GetAllAsync(CancellationToken ct)
    {
        var groups = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseGroupDto>>(groups);
    }

    /// <summary>
    /// Получить группу по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO группы</returns>
    public async Task<ResponseGroupDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var group = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseGroupDto>(group);
    }

    /// <summary>
    /// Создать новую группу
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO созданной группы</returns>
    public async Task<ResponseGroupDto> CreateAsync(CreateGroupDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _repository.ExistsByAsync(dto.Name, null, ct).ConfigureAwait(false))
            throw new UniqueNameConflictException(dto.Name);
        var group = _mapper.Map<Group>(dto);
        await _repository.CreateAsync(group, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseGroupDto>(group);
    }

    /// <summary>
    /// Обновить данные группы
    /// </summary>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="id">Уникальный идентификатор</param>
    /// <returns>DTO обновленной группы</returns>
    public async Task UpdateAsync(int id, UpdateGroupDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _repository.ExistsByAsync(dto.Name, id, ct).ConfigureAwait(false))
            throw new UniqueNameConflictException(dto.Name);
        var group = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        _mapper.Map(dto, group);
        ArgumentNullException.ThrowIfNull(group);
        await _repository.UpdateAsync(group, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удалить группу
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="ct">Токен отмены</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
    }

    #endregion
}