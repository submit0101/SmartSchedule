using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для работы с кабинетами.
/// Управляет кэшированием DTO и инвалидацией при изменениях.
/// </summary>
public class CabinetService : ICabinetService
{
    #region Поля

    private readonly ICabinetRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cache;

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор сервиса кабинетов.
    /// </summary>
    /// <param name="repository">Репозиторий кабинетов.</param>
    /// <param name="mapper">Объект маппинга.</param>
    /// <param name="cache">Сервис кэширования.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если любой из параметров равен <see langword="null"/>.</exception>
    public CabinetService(ICabinetRepository repository, IMapper mapper, ICachingService cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    #endregion

    #region Методы

    /// <summary>
    /// Получить все кабинеты с информацией о зданиях.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список DTO кабинетов.</returns>
    public async Task<List<ResponseCabinetDto>> GetAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "cabinets:all",
            async () =>
            {
                var cabinets = await _repository.GetAllWithBuldingAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ResponseCabinetDto>>(cabinets);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Получить список кабинетов в коротком формате со связью к зданию.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список коротких DTO кабинетов.</returns>
    public async Task<List<ShortCabinetDto>> GetAllShortWithBuilding(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "cabinets:short",
            async () =>
            {
                var cabinets = await _repository.GetAllWithBuldingAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ShortCabinetDto>>(cabinets);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Получить кабинет по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор кабинета.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO кабинета.</returns>
    public async Task<ResponseCabinetDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var cabinet = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseCabinetDto>(cabinet);
    }

    /// <summary>
    /// Создать новый кабинет.
    /// </summary>
    /// <param name="dto">Данные для создания.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO созданного кабинета.</returns>
    public async Task<ResponseCabinetDto> CreateAsync(CreateCabinetDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (await _repository.ExistsAsync(dto.Number, dto.BuildingId, null, ct).ConfigureAwait(false))
            throw new CabinetConflictException(dto.Number, dto.BuildingId);

        var cabinet = _mapper.Map<Cabinet>(dto);
        await _repository.CreateAsync(cabinet, ct).ConfigureAwait(false);

        // Инвалидация кэша
        await _cache.RemoveAsync("cabinets:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:short", ct).ConfigureAwait(false);

        return _mapper.Map<ResponseCabinetDto>(cabinet);
    }

    /// <summary>
    /// Обновить данные кабинета.
    /// </summary>
    /// <param name="id">Уникальный идентификатор кабинета.</param>
    /// <param name="dto">Данные для обновления.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>DTO обновленного кабинета.</returns>
    public async Task UpdateAsync(int id, UpdateCabinetDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (await _repository.ExistsAsync(dto.Number, dto.BuildingId, id, ct).ConfigureAwait(false))
            throw new CabinetConflictException(dto.Number, dto.BuildingId);

        var cabinet = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (cabinet == null)
            throw new ArgumentException("Кабинет не найден", nameof(id));

        _mapper.Map(dto, cabinet);
        await _repository.UpdateAsync(cabinet, ct).ConfigureAwait(false);

        // Инвалидация кэша
        await _cache.RemoveAsync("cabinets:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:short", ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удалить кабинет.
    /// </summary>
    /// <param name="id">Идентификатор кабинета.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("cabinets:short", ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Выполняет поиск и фильтрацию кабинетов.
    /// </summary>
    /// <param name="searchTerm">Поисковый запрос по номеру кабинета.</param>
    /// <param name="buildingNumber">Фильтр по зданию.</param>
    /// <param name="sortBy">Поле сортировки ("number" или "building").</param>
    /// <param name="descending">Направление сортировки.</param>
    /// <returns>Список найденных кабинетов.</returns>
    public async Task<List<ResponseCabinetDto>> SearchCabinetsAsync(
        string? searchTerm,
        int? buildingNumber,
        string? sortBy,
        bool descending)
    {
        var cabinets = await _repository.SearchAsync(searchTerm, buildingNumber, sortBy, descending).ConfigureAwait(false);
        return _mapper.Map<List<ResponseCabinetDto>>(cabinets);
        // Поиск не кэшируется — динамический запрос
    }

    #endregion
}