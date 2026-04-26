using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Constants;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для выполнения аналитических запросов и формирования матриц данных.
/// Не отслеживает сущности (AsNoTracking) для максимальной производительности.
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ReportRepository"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных приложения.</param>
    public ReportRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<List<Lesson>> GetLessonsForReportAsync(CancellationToken ct = default)
    {
        return await _context.Lessons
            .AsNoTracking()
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.Subject)
            .Include(l => l.Cabinet)
                .ThenInclude(c => c!.Building)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<(int TeacherId, int DayId, int SlotId)>> GetBusyMatrixAsync(ReadOnlyCollection<int> teacherIds, int weekTypeId, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(teacherIds);

        const int BothWeeksId = WeekTypeConstants.Both;
        var data = await _context.Lessons
            .AsNoTracking()
            .Where(l => l.TeacherId.HasValue &&
                        teacherIds.Contains(l.TeacherId.Value) &&
                        l.TimeSlotId.HasValue &&
                        (l.WeekTypeId == weekTypeId || l.WeekTypeId == BothWeeksId))
            .Select(l => new
            {
                TeacherId = l.TeacherId!.Value,
                DayId = l.DayOfWeekId,
                SlotId = l.TimeSlotId!.Value
            })
            .Distinct()
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return data
            .Select(d => (d.TeacherId, d.DayId, d.SlotId))
            .ToList();
    }
}