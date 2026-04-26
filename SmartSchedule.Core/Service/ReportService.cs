using AutoMapper;
using SmartSchedul.Core.Exceptions;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Constants;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.ReportDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using System.Collections.ObjectModel;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Сервис для формирования аналитических отчетов и визуализации расписания.
/// </summary>
public class ReportService : IReportService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IMapper _mapper;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ICabinetRepository _cabinetRepository;
    private readonly IWeekDayRepository _weekDayRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGroupRepository _groupRepository;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ReportService"/>.
    /// </summary>
    public ReportService(
        ILessonRepository repository,
        IMapper mapper,
        IReportRepository reportRepository,
        ITimeSlotRepository timeSlotRepository,
        ICabinetRepository cabinetRepository,
        IWeekDayRepository weekDayRepository,
        ITeacherRepository teacherRepository,
        IGroupRepository groupRepository)
    {
        _lessonRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _timeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
        _cabinetRepository = cabinetRepository ?? throw new ArgumentNullException(nameof(cabinetRepository));
        _weekDayRepository = weekDayRepository ?? throw new ArgumentNullException(nameof(weekDayRepository));
        _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
    }

    /// <inheritdoc />
    public async Task<List<TeacherUsageReportDto>> GetTeacherUsageReportAsync(TeacherUsageFilterDto filter, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        const int WorkingDaysPerWeek = 6;

        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        int maxSlotsPerDay = timeSlots.Count;
        if (maxSlotsPerDay == 0) throw new InvalidOperationException("Не настроены тайм-слоты.");

        var allLessons = await _lessonRepository.GetFilteredLessonsAsync(null, filter.DayOfWeekId, null, ct).ConfigureAwait(false);
        var teachers = await _teacherRepository.GetAllAsync(ct).ConfigureAwait(false);

        var allowedDays = filter.DayOfWeekId.HasValue ? new List<int> { filter.DayOfWeekId.Value } : Enumerable.Range(1, WorkingDaysPerWeek).ToList();
        int maxPossibleLessons = filter.DayOfWeekId.HasValue ? maxSlotsPerDay : WorkingDaysPerWeek * maxSlotsPerDay;

        var filteredLessons = allLessons.Where(l => allowedDays.Contains(l.DayOfWeekId));
        if (filter.WeekTypeId.HasValue)
        {
            filteredLessons = filteredLessons.Where(l => l.WeekTypeId == filter.WeekTypeId.Value || l.WeekTypeId == WeekTypeConstants.Both);
        }

        var lessonCounts = filteredLessons.GroupBy(l => l.TeacherId.GetValueOrDefault()).ToDictionary(g => g.Key, g => g.Count());

        return teachers.Select(teacher => {
            int scheduled = lessonCounts.GetValueOrDefault(teacher.Id, 0);
            return new TeacherUsageReportDto
            {
                TeacherFullName = $"{teacher.LastName} {teacher.FirstName} {teacher.MiddleName}",
                TotalLessons = scheduled,
                MaxPossibleLessons = maxPossibleLessons,
                UsagePercentage = maxPossibleLessons > 0 ? (double)Math.Round((decimal)scheduled / maxPossibleLessons * 100m, 2) : 0
            };
        }).OrderByDescending(r => r.UsagePercentage).ToList();
    }

    /// <inheritdoc />
    public async Task<List<TeacherScheduleReportDto>> GetTeacherScheduleAsync(int teacherId, int weekTypeId, CancellationToken ct)
    {
        const int ShowAllWeeksId = 0;
        const int WholeWeekId = 3;
        var teacher = await _teacherRepository.GetWithLessonsByIdAsync(teacherId, ct).ConfigureAwait(false);
        if (teacher == null) throw new ObjectNotFoundException($"Преподаватель с ID {teacherId} не найден.");

        var daysOfWeek = await _weekDayRepository.GetAllAsync(ct).ConfigureAwait(false);
        var timeSlots = (await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false)).OrderBy(ts => ts.SlotNumber).ToList();
        var report = new List<TeacherScheduleReportDto>();

        for (int day = 1; day <= 6; day++)
        {
            var dayName = daysOfWeek.FirstOrDefault(d => d.Id == day)?.Name ?? "День не определен";
            foreach (var slot in timeSlots)
            {
                bool isBusy = teacher.Lessons.Any(l => l.DayOfWeekId == day && l.TimeSlotId == slot.Id &&
                    (weekTypeId == ShowAllWeeksId || l.WeekTypeId == weekTypeId || ((weekTypeId == 1 || weekTypeId == 2) && l.WeekTypeId == WholeWeekId)));
                report.Add(new TeacherScheduleReportDto { DayOfWeekId = day, DayName = dayName, TimeSlotId = slot.Id, TimeSlotDisplay = $"{slot.StartTime:hh\\:mm} - {slot.EndTime:hh\\:mm}", IsBusy = isBusy });
            }
        }
        return report;
    }

    /// <inheritdoc />
    public async Task<List<GroupUsageReportDto>> GetGroupUsageReportAsync(GroupUsageFilterDto filter, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        const int WorkingDaysPerWeek = 6;
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        if (timeSlots.Count == 0) throw new InvalidOperationException("Не настроены тайм-слоты.");

        var allLessons = await _lessonRepository.GetFilteredLessonsAsync(null, filter.DayOfWeekId, null, ct).ConfigureAwait(false);
        var groups = await _groupRepository.GetAllAsync(ct).ConfigureAwait(false);

        int maxPossibleLessons = filter.DayOfWeekId.HasValue ? timeSlots.Count : WorkingDaysPerWeek * timeSlots.Count;
        var allowedDays = filter.DayOfWeekId.HasValue ? new List<int> { filter.DayOfWeekId.Value } : Enumerable.Range(1, WorkingDaysPerWeek).ToList();

        var filteredLessons = allLessons.Where(l => allowedDays.Contains(l.DayOfWeekId));
        if (filter.WeekTypeId.HasValue) filteredLessons = filteredLessons.Where(l => l.WeekTypeId == filter.WeekTypeId.Value || l.WeekTypeId == WeekTypeConstants.Both);

        var scheduledCounts = filteredLessons.Where(l => l.GroupId.HasValue).GroupBy(l => l.GroupId!.Value).ToDictionary(g => g.Key, g => g.Count());

        return groups.Select(group => {
            int scheduled = scheduledCounts.GetValueOrDefault(group.Id, 0);
            return new GroupUsageReportDto { GroupName = group.Name, TotalScheduledLessons = scheduled, MaxPossibleLessons = maxPossibleLessons, UsagePercentage = maxPossibleLessons > 0 ? Math.Round((decimal)scheduled / maxPossibleLessons * 100m, 2) : 0m };
        }).OrderByDescending(r => r.UsagePercentage).ToList();
    }

    /// <inheritdoc />
    public async Task<List<CabinetUsageReportDto>> GetCabinetUsageReportAsync(CabinetUsageFilterDto filter, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(filter);
        const int WorkingDaysPerWeek = 6;
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        if (timeSlots.Count == 0) return new List<CabinetUsageReportDto>();

        int maxPossibleLessons = filter.DayOfWeekId.HasValue ? timeSlots.Count : WorkingDaysPerWeek * timeSlots.Count;
        var cabinets = await _cabinetRepository.GetAllFilteredAsync(filter.BuildingId, ct).ConfigureAwait(false);
        var allLessons = await _lessonRepository.GetFilteredLessonsAsync(null, filter.DayOfWeekId, null, ct).ConfigureAwait(false);

        var lessons = allLessons.Where(l => (!filter.WeekTypeId.HasValue || l.WeekTypeId == filter.WeekTypeId.Value || l.WeekTypeId == WeekTypeConstants.Both) && (!filter.DayOfWeekId.HasValue || l.DayOfWeekId == filter.DayOfWeekId.Value)).ToList();

        return cabinets.Select(cabinet => {
            int lessonCount = lessons.Count(l => l.CabinetId == cabinet.Id);
            return new CabinetUsageReportDto { CabinetNumber = cabinet.Number, BuildingName = cabinet.Building?.Name ?? "Не указано", TotalLessons = lessonCount, UsagePercentage = maxPossibleLessons > 0 ? Math.Round((double)lessonCount / maxPossibleLessons * 100.0, 2) : 0.0 };
        }).OrderByDescending(r => r.UsagePercentage).ToList();
    }

    /// <inheritdoc />
    public async Task<List<CabinetScheduleReportDto>> GetCabinetScheduleAsync(int cabinetId, int weekTypeId, CancellationToken ct)
    {
        const int ShowAllWeeksId = 0;
        var cabinet = await _cabinetRepository.GetWithLessonsByIdAsync(cabinetId, ct).ConfigureAwait(false);
        if (cabinet == null) throw new ObjectNotFoundException($"Кабинет с ID {cabinetId} не найден.");

        var daysOfWeek = await _weekDayRepository.GetAllAsync(ct).ConfigureAwait(false);
        var timeSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        var report = new List<CabinetScheduleReportDto>();

        for (int day = 1; day <= 6; day++)
        {
            var dayName = daysOfWeek.FirstOrDefault(d => d.Id == day)?.Name ?? "Неизвестный день";
            foreach (var slot in timeSlots)
            {
                bool isBusy = cabinet.Lessons.Where(l => l.WeekTypeId == weekTypeId || weekTypeId == ShowAllWeeksId).Any(l => l.DayOfWeekId == day && l.TimeSlotId == slot.Id);
                report.Add(new CabinetScheduleReportDto { DayOfWeekId = day, DayName = dayName, TimeSlotId = slot.Id, TimeSlotDisplay = $"{slot.StartTime} - {slot.EndTime}", IsBusy = isBusy });
            }
        }
        return report;
    }

    /// <inheritdoc />
    public async Task<DynamicReportResultDto> GenerateDynamicReportAsync(DynamicReportFilterDto filter, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var rawLessons = await _reportRepository.GetLessonsForReportAsync(ct).ConfigureAwait(false);
        var flatLessons = _mapper.Map<List<LessonReportFlatDto>>(rawLessons);

        Func<LessonReportFlatDto, string, string> getValue = (l, type) => type switch {
            "Teacher" => l.Teacher,
            "Group" => l.Group,
            "Subject" => l.Subject,
            "Cabinet" => l.Cabinet,
            "Building" => l.Building,
            _ => "Неизвестно"
        };

        var groupedData = flatLessons.GroupBy(l => new { Row = getValue(l, filter.RowGrouping), Col = getValue(l, filter.ColGrouping) })
            .Select(g => new { RowName = g.Key.Row, ColName = g.Key.Col, Hours = g.Count() * 2 }).ToList();

        var result = new DynamicReportResultDto();
        var uniqueColumns = groupedData.Select(x => x.ColName).Distinct().OrderBy(x => x).ToList();
        foreach (var col in uniqueColumns) result.Columns.Add(col);

        foreach (var rowName in groupedData.Select(x => x.RowName).Distinct().OrderBy(x => x))
        {
            var reportRow = new ReportRowDto { RowName = rowName };
            foreach (var col in result.Columns) reportRow.Values.Add(col, groupedData.FirstOrDefault(x => x.RowName == rowName && x.ColName == col)?.Hours ?? 0);
            reportRow.TotalHours = reportRow.Values.Values.Sum();
            result.Rows.Add(reportRow);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<MethodicalWindowReportDto> GenerateMethodicalWindowsReportAsync(ReadOnlyCollection<int> teacherIds, int weekTypeId, CancellationToken ct = default)
    {
        if (teacherIds == null || teacherIds.Count < 2) throw new BusinessException("Для ведомости выберите минимум двух преподавателей.");

        var days = await _weekDayRepository.GetAllAsync(ct).ConfigureAwait(false);
        var slots = (await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false)).OrderBy(s => s.SlotNumber).ToList();
        var busyMatrix = await _reportRepository.GetBusyMatrixAsync(teacherIds, weekTypeId, ct).ConfigureAwait(false);
        var selectedTeachers = (await _teacherRepository.GetAllAsync(ct).ConfigureAwait(false)).Where(t => teacherIds.Contains(t.Id)).ToList();

        var busyHash = teacherIds.ToDictionary(id => id, id => busyMatrix.Where(x => x.TeacherId == id).Select(x => (x.DayId, x.SlotId)).ToHashSet());
        var report = new MethodicalWindowReportDto();

        foreach (var day in days.OrderBy(d => d.Id))
        {
            foreach (var slot in slots)
            {
                var free = selectedTeachers.Where(t => !busyHash[t.Id].Contains((day.Id, slot.Id))).Select(t => $"{t.LastName} {t.FirstName?[0]}.").ToList();
                if (free.Count >= 2) report.Rows.Add(new MethodicalWindowRowDto { DayName = day.Name, TimeDisplay = $"{slot.StartTime:HH\\:mm} - {slot.EndTime:HH\\:mm}", FreeTeachersCount = free.Count, FreeTeachersNames = string.Join(", ", free) });
            }
        }
        report.TotalWindowsFound = report.Rows.Count;
        return report;
    }
}