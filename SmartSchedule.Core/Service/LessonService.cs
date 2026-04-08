using AutoMapper;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Constants;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для управления занятиями, проверки конфликтов расписания и формирования аналитических отчетов.
/// </summary>
public class LessonService : ILessonService
{
    private readonly ILessonRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ICabinetRepository _cabinetRepository;
    private readonly IWeekDayRepository _weekDayRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGroupRepository _groupRepository;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="LessonService"/>.
    /// </summary>
    public LessonService(
        ILessonRepository repository,
        IMapper mapper,
        ITimeSlotRepository timeSlotRepository,
        ICabinetRepository cabinetRepository,
        IWeekDayRepository weekDayRepository,
        ITeacherRepository teacherRepository,
        IGroupRepository groupRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _timeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
        _cabinetRepository = cabinetRepository ?? throw new ArgumentNullException(nameof(cabinetRepository));
        _weekDayRepository = weekDayRepository ?? throw new ArgumentNullException(nameof(weekDayRepository));
        _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
    }

    /// <summary>
    /// Проверяет наличие конфликтов (пересечений по времени, аудитории, преподавателю или группе) для занятия.
    /// </summary>
    /// <param name="lessonId">Идентификатор проверяемого занятия (0, если создается новое).</param>
    /// <param name="targetDayId">Идентификатор дня недели.</param>
    /// <param name="targetTimeId">Идентификатор временного слота.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>DTO с флагами обнаруженных конфликтов.</returns>
    public async Task<ConflictCheckResultDto> CheckConflictAsync(int lessonId, int targetDayId, int targetTimeId, CancellationToken ct)
    {
        var result = new ConflictCheckResultDto();
        var originalLesson = await _repository.GetByIdAsync(lessonId, ct).ConfigureAwait(false);
        if (originalLesson == null) return result;

        const int BothWeeksId = WeekTypeConstants.Both;

        var allLessons = await _repository.GetAllAsync(ct).ConfigureAwait(false);

        var lessonsInTargetSlot = allLessons
            .Where(x => x.TimeSlotId == targetTimeId &&
                        x.DayOfWeekId == targetDayId &&
                        x.Id != lessonId)
            .ToList();

        Func<int, int, bool> isWeekClash = (existingWeekTypeId, newWeekTypeId) =>
            existingWeekTypeId == BothWeeksId ||
            newWeekTypeId == BothWeeksId ||
            existingWeekTypeId == newWeekTypeId;

        foreach (var existingLesson in lessonsInTargetSlot)
        {
            int existWt = existingLesson.WeekTypeId.GetValueOrDefault(0);
            int origWt = originalLesson.WeekTypeId.GetValueOrDefault(0);

            if (isWeekClash(existWt, origWt))
            {
                
                bool isGroupConflict = existingLesson.GroupId == originalLesson.GroupId &&
                                       (existingLesson.Subgroup == null || originalLesson.Subgroup == null || existingLesson.Subgroup == originalLesson.Subgroup);
                if (isGroupConflict) result.IsWeekConflict = true;

                
                if (existingLesson.CabinetId == originalLesson.CabinetId) result.IsCabinetBusy = true;

                
                if (existingLesson.TeacherId == originalLesson.TeacherId) result.IsTeacherBusy = true;
            }

            
            if (result.IsWeekConflict && result.IsTeacherBusy && result.IsCabinetBusy) break;
        }

        return result;
    }

    /// <summary>
    /// Формирует отчет по загруженности преподавателей на основе заданных фильтров.
    /// </summary>
    /// <param name="filter">Параметры фильтрации (день недели, тип недели).</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список с данными о загрузке каждого преподавателя.</returns>
    public async Task<List<TeacherUsageReportDto>> GetTeacherUsageReportAsync(TeacherUsageFilterDto filter, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        const int WorkingDaysPerWeek = 6;

        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        int maxSlotsPerDay = timeSlots.Count;
        if (maxSlotsPerDay == 0) throw new InvalidOperationException("Ошибка: Не настроены тайм-слоты.");

        var allLessons = await _repository.GetFilteredLessonsAsync(
            weekTypeId: null,
            dayOfWeekId: filter.DayOfWeekId,
            cabinetId: null,
            ct: ct).ConfigureAwait(false);

        var teachers = await _teacherRepository.GetAllAsync(ct).ConfigureAwait(false);

        var allowedDays = filter.DayOfWeekId.HasValue
            ? new List<int> { filter.DayOfWeekId.Value }
            : Enumerable.Range(1, WorkingDaysPerWeek).ToList();

        int maxPossibleLessons = filter.DayOfWeekId.HasValue
            ? maxSlotsPerDay
            : WorkingDaysPerWeek * maxSlotsPerDay;

        if (maxPossibleLessons <= 0) return new List<TeacherUsageReportDto>();

        var filteredLessons = allLessons.Where(l => allowedDays.Contains(l.DayOfWeekId));

        if (filter.WeekTypeId.HasValue)
        {
            int wt = filter.WeekTypeId.Value;
            filteredLessons = filteredLessons.Where(l =>
                l.WeekTypeId == wt || l.WeekTypeId == WeekTypeConstants.Both);
        }

        var lessonCounts = filteredLessons
            .GroupBy(l => l.TeacherId.GetValueOrDefault())
            .ToDictionary(g => g.Key, g => g.Count());

        var report = new List<TeacherUsageReportDto>();
        foreach (var teacher in teachers)
        {
            int scheduled = lessonCounts.GetValueOrDefault(teacher.Id, 0);
            decimal percentage = maxPossibleLessons > 0
                ? Math.Round((decimal)scheduled / maxPossibleLessons * 100m, 2)
                : 0m;

            report.Add(new TeacherUsageReportDto
            {
                TeacherFullName = $"{teacher.LastName} {teacher.FirstName} {teacher.MiddleName}",
                TotalLessons = scheduled,
                MaxPossibleLessons = maxPossibleLessons,
                UsagePercentage = (double)percentage
            });
        }

        return report.OrderByDescending(r => r.UsagePercentage).ToList();
    }

    /// <summary>
    /// Получает расписание занятий конкретного преподавателя для визуализации в виде сетки.
    /// </summary>
    /// <param name="teacherId">Идентификатор преподавателя.</param>
    /// <param name="weekTypeId">Тип недели (Числитель/Знаменатель).</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список слотов с отметками о занятости.</returns>
    public async Task<List<TeacherScheduleReportDto>> GetTeacherScheduleAsync(int teacherId, int weekTypeId, CancellationToken ct)
    {
        const int ShowAllWeeksId = 0;
        const int WholeWeekId = 3;

        var teacher = await _teacherRepository.GetWithLessonsByIdAsync(teacherId, ct).ConfigureAwait(false);
        var daysOfWeek = await _weekDayRepository.GetAllAsync(ct).ConfigureAwait(false);
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);

        if (teacher == null) throw new ObjectNotFoundException($"Преподаватель с ID {teacherId} не найден.");

        var report = new List<TeacherScheduleReportDto>();
        var orderedSlots = timeSlots.OrderBy(ts => ts.SlotNumber).ToList();

        for (int day = 1; day <= 6; day++)
        {
            var dayOfWeek = daysOfWeek.FirstOrDefault(d => d.Id == day);
            var dayName = dayOfWeek?.Name ?? "День не определен";

            foreach (var slot in orderedSlots)
            {
                var isBusy = teacher.Lessons.Any(l =>
                        l.DayOfWeekId == day &&
                        l.TimeSlotId == slot.Id &&
                        (
                            weekTypeId == ShowAllWeeksId ||
                            l.WeekTypeId == weekTypeId ||
                            ((weekTypeId == 1 || weekTypeId == 2) && l.WeekTypeId == WholeWeekId)
                        )
                    );

                report.Add(new TeacherScheduleReportDto
                {
                    DayOfWeekId = day,
                    DayName = dayName,
                    TimeSlotId = slot.Id,
                    TimeSlotDisplay = $"{slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}",
                    IsBusy = isBusy
                });
            }
        }
        return report;
    }

    /// <summary>
    /// Получает список всех занятий в системе.
    /// </summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список DTO занятий.</returns>
    public async Task<List<ResponseLessonDto>> GetAllAsync(CancellationToken ct)
    {
        var lessons = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseLessonDto>>(lessons);
    }

    /// <summary>
    /// Получает информацию о занятии по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор занятия.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>DTO занятия.</returns>
    public async Task<ResponseLessonDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var lesson = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseLessonDto>(lesson);
    }

    /// <summary>
    /// Возвращает список свободных кабинетов для указанного временного слота.
    /// </summary>
    /// <param name="dayOfWeekId">День недели.</param>
    /// <param name="timeSlotId">Временной слот.</param>
    /// <param name="weekTypeId">Тип недели.</param>
    /// <param name="buildingId">Фильтр по корпусу (необязательно).</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список свободных аудиторий.</returns>
    public async Task<List<ResponseCabinetDto>> GetAvailableCabinetsAsync(int dayOfWeekId, int timeSlotId, int weekTypeId, int? buildingId = null, CancellationToken ct = default)
    {
        var filteredCabinets = await _cabinetRepository.GetAllFilteredAsync(buildingId, ct).ConfigureAwait(false);
        const int BothWeeksId = WeekTypeConstants.Both;
        var allLessons = await _repository.GetAllAsync(ct).ConfigureAwait(false);
        var busyCabinetIds = new HashSet<int>();

        foreach (var l in allLessons)
        {
            if (l.CabinetId.HasValue &&
                l.DayOfWeekId == dayOfWeekId &&
                l.TimeSlotId == timeSlotId &&
                (l.WeekTypeId == weekTypeId || l.WeekTypeId == BothWeeksId))
            {
                busyCabinetIds.Add(l.CabinetId.Value);
            }
        }

        var availableCabinets = filteredCabinets.Where(c => !busyCabinetIds.Contains(c.Id)).ToList();
        return _mapper.Map<List<ResponseCabinetDto>>(availableCabinets);
    }

    /// <summary>
    /// Создает новое занятие после проверки на конфликты.
    /// </summary>
    /// <param name="dto">DTO с данными для создания.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>DTO созданного занятия.</returns>
    /// <exception cref="ScheduleConflictException">Если обнаружено пересечение в расписании.</exception>
    public async Task<ResponseLessonDto> CreateAsync(CreateLessonDto dto, CancellationToken ct)
    {
        var lesson = _mapper.Map<Lesson>(dto);
        await CheckForScheduleConflict(lesson, ct).ConfigureAwait(false);
        await _repository.CreateAsync(lesson, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseLessonDto>(lesson);
    }

    /// <summary>
    /// Обновляет существующее занятие после проверки на конфликты.
    /// </summary>
    /// <param name="id">Идентификатор занятия.</param>
    /// <param name="dto">DTO с обновленными данными.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <exception cref="ArgumentException">Если занятие не найдено.</exception>
    /// <exception cref="ScheduleConflictException">Если обнаружено пересечение в расписании.</exception>
    public async Task UpdateAsync(int id, UpdateLessonDto dto, CancellationToken ct)
    {
        var lesson = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (lesson == null) throw new ArgumentException("Занятие не найдено");
        _mapper.Map(dto, lesson);
        await CheckForScheduleConflict(lesson, ct).ConfigureAwait(false);
        await _repository.UpdateAsync(lesson, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Удаляет занятие по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор занятия.</param>
    /// <param name="ct">Токен отмены операции.</param>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Получает все занятия для указанной группы.
    /// </summary>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список занятий группы.</returns>
    public async Task<List<ResponseLessonDto>> GetByGroupIdAsync(int groupId, CancellationToken ct)
    {
        var lessons = await _repository.GetByGroupIdAsync(groupId, ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseLessonDto>>(lessons);
    }

    /// <summary>
    /// Получает все занятия для указанного преподавателя.
    /// </summary>
    /// <param name="teacherId">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список занятий преподавателя.</returns>
    public async Task<List<ResponseLessonDto>> GetByTeacherIdAsync(int teacherId, CancellationToken ct)
    {
        var lessons = await _repository.GetByTeacherIdAsync(teacherId, ct).ConfigureAwait(false);
        return _mapper.Map<List<ResponseLessonDto>>(lessons);
    }

    /// <summary>
    /// Получает структурированное расписание (группированное по дням и слотам) для группы.
    /// </summary>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Словарь, где ключ - день недели, значение - список занятий.</returns>
    public async Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByGroupIdAsync(int groupId, CancellationToken ct)
    {
        var lessons = await _repository.GetByGroupIdAsync(groupId, ct).ConfigureAwait(false);
        var lessonDtos = _mapper.Map<List<ResponseLessonDto>>(lessons);
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        var orderedTimeSlots = timeSlots.OrderBy(ts => ts.SlotNumber).ToList();
        var schedule = new Dictionary<int, List<StructuredLessonDto>>();

        for (int day = 1; day <= 6; day++)
        {
            var dailyLessons = new List<StructuredLessonDto>();
            foreach (var slot in orderedTimeSlots)
            {
                var lessonsForSlot = lessonDtos.Where(l => l.DayOfWeekId == day && l.TimeSlotId == slot.Id).ToList();
                dailyLessons.Add(new StructuredLessonDto { Lessons = lessonsForSlot, TimeSlotDisplay = $"{slot.StartTime} - {slot.EndTime}" });
            }
            schedule[day] = dailyLessons;
        }
        return schedule;
    }

    /// <summary>
    /// Получает структурированное расписание (группированное по дням и слотам) для преподавателя.
    /// </summary>
    /// <param name="teacherId">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Словарь, где ключ - день недели, значение - список занятий.</returns>
    public async Task<Dictionary<int, List<StructuredLessonDto>>> GetStructuredScheduleByTeacherIdAsync(int teacherId, CancellationToken ct)
    {
        var lessons = await _repository.GetByTeacherIdAsync(teacherId, ct).ConfigureAwait(false);
        var lessonDtos = _mapper.Map<List<ResponseLessonDto>>(lessons);
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        var orderedTimeSlots = timeSlots.OrderBy(ts => ts.SlotNumber).ToList();
        var schedule = new Dictionary<int, List<StructuredLessonDto>>();

        for (int day = 1; day <= 6; day++)
        {
            var dailyLessons = new List<StructuredLessonDto>();
            foreach (var slot in orderedTimeSlots)
            {
                var lessonsForSlot = lessonDtos.Where(l => l.DayOfWeekId == day && l.TimeSlotId == slot.Id).ToList();
                dailyLessons.Add(new StructuredLessonDto { Lessons = lessonsForSlot, TimeSlotDisplay = $"{slot.StartTime} - {slot.EndTime}" });
            }
            schedule[day] = dailyLessons;
        }
        return schedule;
    }

    /// <summary>
    /// Формирует отчет по загруженности учебных групп на основе фильтров.
    /// </summary>
    /// <param name="filter">Параметры фильтрации.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список с данными о загрузке каждой группы.</returns>
    public async Task<List<GroupUsageReportDto>> GetGroupUsageReportAsync(GroupUsageFilterDto filter, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        const int WorkingDaysPerWeek = 6;
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        int maxSlotsPerDay = timeSlots.Count;
        if (maxSlotsPerDay == 0) throw new InvalidOperationException("Не настроены тайм-слоты в системе.");

        var allLessons = await _repository.GetFilteredLessonsAsync(null, filter.DayOfWeekId, null, ct).ConfigureAwait(false);
        var groups = await _groupRepository.GetAllAsync(ct).ConfigureAwait(false);

        var allowedDays = filter.DayOfWeekId.HasValue
            ? new List<int> { filter.DayOfWeekId.Value }
            : Enumerable.Range(1, WorkingDaysPerWeek).ToList();

        int maxPossibleLessons = filter.DayOfWeekId.HasValue
            ? maxSlotsPerDay
            : WorkingDaysPerWeek * maxSlotsPerDay;

        if (maxPossibleLessons <= 0) return new List<GroupUsageReportDto>();

        var filteredLessons = allLessons.Where(l => allowedDays.Contains(l.DayOfWeekId));

        if (filter.WeekTypeId.HasValue)
        {
            int wt = filter.WeekTypeId.Value;
            filteredLessons = filteredLessons.Where(l => l.WeekTypeId == wt || l.WeekTypeId == WeekTypeConstants.Both);
        }

        var scheduledCounts = filteredLessons
            .Where(l => l.GroupId.HasValue)
            .GroupBy(l => l.GroupId.GetValueOrDefault())
            .ToDictionary(g => g.Key, g => g.Count());

        var report = new List<GroupUsageReportDto>();
        foreach (var group in groups)
        {
            int scheduled = scheduledCounts.GetValueOrDefault(group.Id, 0);
            decimal percentage = maxPossibleLessons > 0
                ? Math.Round((decimal)scheduled / maxPossibleLessons * 100m, 2)
                : 0m;

            report.Add(new GroupUsageReportDto
            {
                GroupName = group.Name,
                TotalScheduledLessons = scheduled,
                MaxPossibleLessons = maxPossibleLessons,
                UsagePercentage = percentage
            });
        }
        return report.OrderByDescending(r => r.UsagePercentage).ToList();
    }

    /// <summary>
    /// Формирует отчет по загруженности аудиторий (кабинетов) на основе фильтров.
    /// </summary>
    /// <param name="filter">Параметры фильтрации (день, тип недели, корпус).</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список с данными о загрузке каждого кабинета.</returns>
    public async Task<List<CabinetUsageReportDto>> GetCabinetUsageReportAsync(CabinetUsageFilterDto filter, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        const int WorkingDaysPerWeek = 6;
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        int maxSlotsPerDay = timeSlots.Count;
        if (maxSlotsPerDay == 0) return new List<CabinetUsageReportDto>();

        int maxPossibleLessons = filter.DayOfWeekId.HasValue
            ? maxSlotsPerDay
            : WorkingDaysPerWeek * maxSlotsPerDay;

        var cabinets = await _cabinetRepository.GetAllFilteredAsync(filter.BuildingId, ct).ConfigureAwait(false);

        var allLessons = await _repository.GetFilteredLessonsAsync(null, filter.DayOfWeekId, null, ct).ConfigureAwait(false);

        var lessons = allLessons.Where(l =>
        {
            bool weekMatch = !filter.WeekTypeId.HasValue
                             || l.WeekTypeId == filter.WeekTypeId.Value
                             || l.WeekTypeId == WeekTypeConstants.Both;

            bool dayMatch = !filter.DayOfWeekId.HasValue
                            || l.DayOfWeekId == filter.DayOfWeekId.Value;

            return weekMatch && dayMatch;
        }).ToList();

        var report = new List<CabinetUsageReportDto>();
        foreach (var cabinet in cabinets)
        {
            var lessonCount = lessons.Count(l => l.CabinetId == cabinet.Id);
            double usagePercentage = maxPossibleLessons > 0 ? (double)lessonCount / maxPossibleLessons * 100.0 : 0.0;

            report.Add(new CabinetUsageReportDto
            {
                CabinetNumber = cabinet.Number,
                BuildingName = cabinet.Building?.Name ?? "Не указано",
                TotalLessons = lessonCount,
                UsagePercentage = Math.Round(usagePercentage, 2)
            });
        }
        return report.OrderByDescending(r => r.UsagePercentage).ToList();
    }

    /// <summary>
    /// Получает расписание занятий конкретного кабинета для визуализации в виде сетки.
    /// </summary>
    /// <param name="cabinetId">Идентификатор кабинета.</param>
    /// <param name="weekTypeId">Тип недели.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список слотов с отметками о занятости.</returns>
    public async Task<List<CabinetScheduleReportDto>> GetCabinetScheduleAsync(int cabinetId, int weekTypeId, CancellationToken ct)
    {
        const int ShowAllWeeksId = 0;
        var cabinet = await _cabinetRepository.GetWithLessonsByIdAsync(cabinetId, ct).ConfigureAwait(false);
        var daysOfWeek = await _weekDayRepository.GetAllAsync(ct).ConfigureAwait(false);

        if (cabinet == null) throw new ObjectNotFoundException($"Кабинет с ID {cabinetId} не найден.");
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        var report = new List<CabinetScheduleReportDto>();

        for (int day = 1; day <= 6; day++)
        {
            var dayOfWeek = daysOfWeek.FirstOrDefault(d => d.Id == day);
            var dayName = dayOfWeek?.Name ?? "Неизвестный день";

            foreach (var slot in timeSlots)
            {
                var isBusy = cabinet.Lessons
                    .Where(l => l.WeekTypeId == weekTypeId || weekTypeId == ShowAllWeeksId)
                    .Any(l => l.DayOfWeekId == day && l.TimeSlotId == slot.Id);

                report.Add(new CabinetScheduleReportDto
                {
                    DayOfWeekId = day,
                    DayName = dayName,
                    TimeSlotId = slot.Id,
                    TimeSlotDisplay = $"{slot.StartTime} - {slot.EndTime}",
                    IsBusy = isBusy
                });
            }
        }
        return report;
    }

    /// <summary>
    /// Внутренний метод для проверки конфликтов в расписании перед сохранением.
    /// Бросает исключение при обнаружении конфликта.
    /// </summary>
    /// <param name="lesson">Проверяемое занятие.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <exception cref="ScheduleConflictException">Если конфликт найден.</exception>
    private async Task CheckForScheduleConflict(Lesson lesson, CancellationToken ct)
    {
        const int BothWeeksId = WeekTypeConstants.Both;
        var allLessons = await _repository.GetAllAsync(ct).ConfigureAwait(false);

        var hasConflict = allLessons.Any(existingLesson =>
        existingLesson.Id != lesson.Id &&
        existingLesson.TimeSlotId == lesson.TimeSlotId &&
        existingLesson.DayOfWeekId == lesson.DayOfWeekId &&
        (

            (existingLesson.GroupId == lesson.GroupId && (existingLesson.Subgroup == null || lesson.Subgroup == null || existingLesson.Subgroup == lesson.Subgroup)) ||

            existingLesson.TeacherId == lesson.TeacherId ||
            existingLesson.CabinetId == lesson.CabinetId
        ) &&
        (
            existingLesson.WeekTypeId == BothWeeksId ||
            lesson.WeekTypeId == BothWeeksId ||
            existingLesson.WeekTypeId == lesson.WeekTypeId
        )
        );

        if (hasConflict) throw new ScheduleConflictException();
    }
}