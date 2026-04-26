using AutoMapper;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Constants;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для управления занятиями и контроля конфликтов расписания.
/// </summary>
public class LessonService : ILessonService
{
    private readonly ILessonRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ICabinetRepository _cabinetRepository;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="LessonService"/>.
    /// </summary>
    public LessonService(ILessonRepository repository, IMapper mapper, ITimeSlotRepository timeSlotRepository, ICabinetRepository cabinetRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _timeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
        _cabinetRepository = cabinetRepository ?? throw new ArgumentNullException(nameof(cabinetRepository));
    }

    /// <inheritdoc />
    public async Task<List<ResponseLessonDto>> GetAllAsync(CancellationToken ct) =>
        _mapper.Map<List<ResponseLessonDto>>(await _repository.GetAllAsync(ct).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<ResponseLessonDto> GetByIdAsync(int id, CancellationToken ct) =>
        _mapper.Map<ResponseLessonDto>(await _repository.GetByIdAsync(id, ct).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<List<ResponseLessonDto>> GetByGroupIdAsync(int groupId, CancellationToken ct) =>
        _mapper.Map<List<ResponseLessonDto>>(await _repository.GetByGroupIdAsync(groupId, ct).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<List<ResponseLessonDto>> GetByTeacherIdAsync(int teacherId, CancellationToken ct) =>
        _mapper.Map<List<ResponseLessonDto>>(await _repository.GetByTeacherIdAsync(teacherId, ct).ConfigureAwait(false));

    /// <inheritdoc />
    public async Task<ResponseLessonDto> CreateAsync(CreateLessonDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var lesson = _mapper.Map<Lesson>(dto);
        await CheckForScheduleConflict(lesson, ct).ConfigureAwait(false);
        await _repository.CreateAsync(lesson, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseLessonDto>(lesson);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int id, UpdateLessonDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var lesson = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false) ?? throw new ArgumentException("Занятие не найдено");
        _mapper.Map(dto, lesson);
        await CheckForScheduleConflict(lesson, ct).ConfigureAwait(false);
        await _repository.UpdateAsync(lesson, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken ct) =>
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<List<ResponseCabinetDto>> GetAvailableCabinetsAsync(int dayOfWeekId, int timeSlotId, int weekTypeId, int? buildingId = null, CancellationToken ct = default)
    {
        var filteredCabinets = await _cabinetRepository.GetAllFilteredAsync(buildingId, ct).ConfigureAwait(false);
        var lessonsInSlot = await _repository.GetLessonsBySlotAsync(dayOfWeekId, timeSlotId, ct).ConfigureAwait(false);
        var busyIds = lessonsInSlot.Where(l => l.CabinetId.HasValue && (l.WeekTypeId == weekTypeId || l.WeekTypeId == WeekTypeConstants.Both)).Select(l => l.CabinetId!.Value).ToHashSet();
        return _mapper.Map<List<ResponseCabinetDto>>(filteredCabinets.Where(c => !busyIds.Contains(c.Id)));
    }

    /// <inheritdoc />
    public async Task<ConflictCheckResultDto> CheckConflictAsync(int lessonId, int targetDayId, int targetTimeId, CancellationToken ct)
    {
        var result = new ConflictCheckResultDto();
        var originalLesson = await _repository.GetByIdAsync(lessonId, ct).ConfigureAwait(false);
        if (originalLesson == null) return result;

        var otherLessons = (await _repository.GetLessonsBySlotAsync(targetDayId, targetTimeId, ct).ConfigureAwait(false)).Where(x => x.Id != lessonId);
        foreach (var existing in otherLessons)
        {
            if (existing.WeekTypeId == WeekTypeConstants.Both || originalLesson.WeekTypeId == WeekTypeConstants.Both || existing.WeekTypeId == originalLesson.WeekTypeId)
            {
                if (existing.GroupId == originalLesson.GroupId && (existing.Subgroup == null || originalLesson.Subgroup == null || existing.Subgroup == originalLesson.Subgroup)) result.IsWeekConflict = true;
                if (existing.CabinetId == originalLesson.CabinetId) result.IsCabinetBusy = true;
                if (existing.TeacherId == originalLesson.TeacherId) result.IsTeacherBusy = true;
            }
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByGroupIdAsync(int groupId, CancellationToken ct) =>
        await BuildStructuredSchedule(() => _repository.GetByGroupIdAsync(groupId, ct), ct).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByTeacherIdAsync(int teacherId, CancellationToken ct) =>
        await BuildStructuredSchedule(() => _repository.GetByTeacherIdAsync(teacherId, ct), ct).ConfigureAwait(false);

    private async Task<Dictionary<int, List<StructuredLessonDto>>> BuildStructuredSchedule(Func<Task<List<Lesson>>> getLessons, CancellationToken ct)
    {
        var lessonDtos = _mapper.Map<List<ResponseLessonDto>>(await getLessons().ConfigureAwait(false));
        var slots = (await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false)).OrderBy(ts => ts.SlotNumber).ToList();
        var schedule = new Dictionary<int, List<StructuredLessonDto>>();
        for (int day = 1; day <= 6; day++) schedule[day] = slots.Select(s => new StructuredLessonDto { Lessons = lessonDtos.Where(l => l.DayOfWeekId == day && l.TimeSlotId == s.Id).ToList(), TimeSlotDisplay = $"{s.StartTime} - {s.EndTime}" }).ToList();
        return schedule;
    }

    private async Task CheckForScheduleConflict(Lesson lesson, CancellationToken ct)
    {
        if (!lesson.TimeSlotId.HasValue) return;
        var existing = await _repository.GetLessonsBySlotAsync(lesson.DayOfWeekId, lesson.TimeSlotId.Value, ct).ConfigureAwait(false);
        if (existing.Any(e => e.Id != lesson.Id && (e.WeekTypeId == WeekTypeConstants.Both || lesson.WeekTypeId == WeekTypeConstants.Both || e.WeekTypeId == lesson.WeekTypeId) && (e.TeacherId == lesson.TeacherId || e.CabinetId == lesson.CabinetId || (e.GroupId == lesson.GroupId && (e.Subgroup == null || lesson.Subgroup == null || e.Subgroup == lesson.Subgroup))))) throw new ScheduleConflictException();
    }

    /// <inheritdoc />
    public async Task UpdateBatchAsync(IReadOnlyCollection<UpdateLessonDto> dtos, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dtos);

        var lessons = new List<Lesson>();
        foreach (var dto in dtos)
        {
            var l = await _repository.GetByIdAsync(dto.Id, ct).ConfigureAwait(false) ?? throw new ObjectNotFoundException($"Занятие с ID {dto.Id} не найдено");
            _mapper.Map(dto, l);
            lessons.Add(l);
        }
        await _repository.UpdateBatchAsync(lessons, ct).ConfigureAwait(false);
    }
}