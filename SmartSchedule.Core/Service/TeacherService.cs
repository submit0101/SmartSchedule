using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для работы с преподавателями.
/// Управляет кэшированием DTO и инвалидацией при изменениях.
/// </summary>
public class TeacherService : ITeacherService
{
    #region Поля

    private readonly ITeacherRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cache;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса преподавателей.
    /// </summary>
    /// <param name="repository">Репозиторий преподавателей.</param>
    /// <param name="mapper">Объект маппинга.</param>
    /// <param name="cache">Сервис кэширования.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если любой из параметров равен <see langword="null"/>.</exception>
    public TeacherService(ITeacherRepository repository, IMapper mapper, ICachingService cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить всех преподавателей в коротком виде.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список DTO преподавателей.</returns>
    public async Task<List<ShortTeacherDto>> GetShortAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "teachers:short",
            async () =>
            {
                var teachers = await _repository.GetAllShorts(ct).ConfigureAwait(false);
                return _mapper.Map<List<ShortTeacherDto>>(teachers);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Получить всех преподавателей с информацией о должностях.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список DTO преподавателей.</returns>
    public async Task<List<ResponseTeacherDto>> GetAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "teachers:all",
            async () =>
            {
                var teachers = await _repository.GetAllWithPositonAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ResponseTeacherDto>>(teachers);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Получить преподавателя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO преподавателя.</returns>
    public async Task<ResponseTeacherDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var teacher = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseTeacherDto>(teacher);
    }

    /// <summary>
    /// Создать нового преподавателя.
    /// </summary>
    /// <param name="dto">Данные для создания.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO созданного преподавателя.</returns>
    public async Task<ResponseTeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var teacher = _mapper.Map<Teacher>(dto);
        await _repository.CreateAsync(teacher, ct).ConfigureAwait(false);

        // Инвалидация кэша
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);

        return _mapper.Map<ResponseTeacherDto>(teacher);
    }

    /// <summary>
    /// Обновить данные преподавателя.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="dto">Данные для обновления.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO обновленного преподавателя.</returns>
    public async Task UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var teacher = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (teacher == null)
            throw new ArgumentException("Преподаватель не найден", nameof(id));

        _mapper.Map(dto, teacher);
        await _repository.UpdateAsync(teacher, ct).ConfigureAwait(false);

        // Инвалидация кэша
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удалить преподавателя.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Выполняет поиск преподавателей по ФИО и фильтрацию по должности.
    /// </summary>
    /// <param name="search">Поисковая строка (ФИО).</param>
    /// <param name="positionId">Идентификатор должности.</param>
    /// <returns>Список найденных преподавателей.</returns>
    public async Task<List<ResponseTeacherDto>> SearchTeachersAsync(string? search, int? positionId)
    {
        var teachers = await _repository.SearchAsync(search, positionId).ConfigureAwait(false);
        return _mapper.Map<List<ResponseTeacherDto>>(teachers);
        // Поиск не кэшируется — динамический запрос
    }

    #endregion
}