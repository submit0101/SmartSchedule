using AutoMapper;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Реализация сервиса для работы с преподавателями.
/// </summary>
public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cache;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса.
    /// </summary>
    /// <param name="repository">Репозиторий преподавателей.</param>
    /// <param name="mapper">Маппер объектов.</param>
    /// <param name="cache">Сервис кэширования.</param>
    public TeacherService(ITeacherRepository repository, IMapper mapper, ICachingService cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc/>
    public async Task<List<ShortTeacherDto>> GetShortAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "teachers:short",
            async () =>
            {
                var teachers = await _repository.GetAllAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ShortTeacherDto>>(teachers);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<ResponseTeacherDto>> GetAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "teachers:all",
            async () =>
            {
                var teachers = await _repository.GetAllAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ResponseTeacherDto>>(teachers);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ResponseTeacherDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var teacher = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseTeacherDto>(teacher);
    }

    /// <inheritdoc/>
    public async Task<ResponseTeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var teacher = _mapper.Map<Teacher>(dto);
        await _repository.CreateAsync(teacher, ct).ConfigureAwait(false);

        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);

        return _mapper.Map<ResponseTeacherDto>(teacher);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var teacher = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (teacher == null)
            throw new ArgumentException("Преподаватель не найден", nameof(id));

        _mapper.Map(dto, teacher);
        await _repository.UpdateAsync(teacher, ct).ConfigureAwait(false);

        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<ResponseTeacherDto>> SearchTeachersAsync(string? search)
    {
        var teachers = await _repository.SearchAsync(search).ConfigureAwait(false);
        return _mapper.Map<List<ResponseTeacherDto>>(teachers);
    }
}