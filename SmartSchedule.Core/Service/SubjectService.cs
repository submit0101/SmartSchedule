using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.SubjectDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для работы с предметами
/// </summary>
public class SubjectService : ISubjectService
{
    #region Поля

    private readonly ISubjectRepository _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса предметов
    /// </summary>
    /// <param name="repository">Репозиторий предметов</param>
    /// <param name="mapper">Объект маппинга</param>
    /// <exception cref="ArgumentNullException">Если repository или mapper равны null</exception>
    public SubjectService(ISubjectRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить все предметы
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список DTO предметов</returns>
    public async Task<List<ResponseSubjectDto>> GetAllAsync(CancellationToken ct)
    {
        var subjects = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseSubjectDto>>(subjects);
    }

    /// <summary>
    /// Получить предмет по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO предмета</returns>
    public async Task<ResponseSubjectDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var subject = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseSubjectDto>(subject);
    }

    /// <summary>
    /// Создать новый предмет
    /// </summary>
    /// <param name="dto">Данные для создания</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>DTO созданного предмета</returns>
    public async Task<ResponseSubjectDto> CreateAsync(CreateSubjectDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _repository.ExistsByNameAsync(dto.Title, null, ct).ConfigureAwait(false))
            throw new UniqueNameConflictException(dto.Title);
        var subject = _mapper.Map<Subject>(dto);
        await _repository.CreateAsync(subject, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseSubjectDto>(subject);
    }

    /// <summary>
    /// Обновить данные предмета
    /// </summary>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    /// <param name="id">уникальный идентификатор</param>
    /// <returns>DTO обновленного предмета</returns>
    public async Task UpdateAsync(int id, UpdateSubjectDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _repository.ExistsByNameAsync(dto.Title, id, ct).ConfigureAwait(false))
            throw new UniqueNameConflictException(dto.Title);
        var subject = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        _mapper.Map(dto, subject);
        ArgumentNullException.ThrowIfNull(subject);
        await _repository.UpdateAsync(subject, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удалить предмет
    /// </summary>
    /// <param name="id">Идентификатор предмета</param>
    /// <param name="ct">Токен отмены</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
    }

    #endregion
}