using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Constants;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с сущностями занятий с точечным кэшированием.
/// </summary>
public class LessonRepository : BaseRepository<Lesson, int, AppDbContext>, ILessonRepository
{
    private readonly DbSet<Lesson> _lessons;
    private readonly ICachingService _cache;
    private readonly AppDbContext _context;

    private static bool UseCache = true;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="LessonRepository"/>.
    /// </summary>
    public LessonRepository(AppDbContext context, ICachingService cache) : base(context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(cache, nameof(cache));

        _context = context;
        _lessons = context.Set<Lesson>();
        _cache = cache;
    }

    #region Чтение данных (GET)

    /// <inheritdoc />
    public override async Task<List<Lesson>> GetAllAsync(CancellationToken ct = default)
    {
        return await base.GetAllAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<Lesson>> GetByGroupIdAsync(int groupId, CancellationToken ct = default)
    {
        if (!UseCache)
        {
            return await GetByGroupFromDb(groupId, ct).ConfigureAwait(false);
        }

        var key = $"lessons:group:{groupId}";

        return await _cache.GetOrSetAsync(
            key,
            () => GetByGroupFromDb(groupId, ct),
            TimeSpan.FromMinutes(10),
            ct
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<Lesson>> GetByTeacherIdAsync(int teacherId, CancellationToken ct = default)
    {
        if (!UseCache)
        {
            return await GetByTeacherFromDb(teacherId, ct).ConfigureAwait(false);
        }

        var key = $"lessons:teacher:{teacherId}";

        return await _cache.GetOrSetAsync(
            key,
            () => GetByTeacherFromDb(teacherId, ct),
            TimeSpan.FromMinutes(10),
            ct
        ).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<List<Lesson>> GetFilteredLessonsAsync(
        int? weekTypeId,
        int? dayOfWeekId,
        int? cabinetId,
        CancellationToken ct = default)
    {
        var query = _lessons.AsQueryable();

        if (weekTypeId.HasValue && weekTypeId.Value != WeekTypeConstants.Both)
        {
            var wt = weekTypeId.Value;
            query = query.Where(l => l.WeekTypeId == wt);
        }

        if (dayOfWeekId.HasValue)
        {
            var day = dayOfWeekId.Value;
            query = query.Where(l => l.DayOfWeekId == day);
        }

        if (cabinetId.HasValue)
        {
            var cab = cabinetId.Value;
            query = query.Where(l => l.CabinetId == cab);
        }

        return await query.ToListAsync(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<HashSet<int>> GetBusyCabinetIdsAsync(
        int dayOfWeekId,
        int timeSlotId,
        int weekTypeId,
        CancellationToken ct = default)
    {
        const int BothWeeksId = WeekTypeConstants.Both;

        var busyIds = await _lessons
            .AsNoTracking()
            .Where(l => l.DayOfWeekId == dayOfWeekId &&
                        l.TimeSlotId == timeSlotId &&
                        (l.WeekTypeId == weekTypeId || l.WeekTypeId == BothWeeksId) &&
                        l.CabinetId != null)
            .Select(l => l.CabinetId!.Value)
            .Distinct()
            .ToHashSetAsync(ct)
            .ConfigureAwait(false);

        return busyIds;
    }

    /// <inheritdoc />
    public async Task<List<Lesson>> GetLessonsBySlotAsync(int dayId, int timeId, CancellationToken ct = default)
    {
        return await _lessons
            .AsNoTracking()
            .Include(l => l.Subject)
            .Where(l => l.DayOfWeekId == dayId && l.TimeSlotId == timeId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    #endregion

    #region Изменение данных (CREATE / UPDATE / DELETE)

    /// <inheritdoc />
    public override async Task<Lesson> CreateAsync(Lesson lesson, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(lesson, nameof(lesson));

        var result = await base.CreateAsync(lesson, ct).ConfigureAwait(false);
        await CleanCacheForLessonAsync(lesson, ct).ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc />
    public override async Task UpdateAsync(Lesson entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await base.UpdateAsync(entity, ct).ConfigureAwait(false);
        await CleanCacheForLessonAsync(entity, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task DeleteByIdAsync(int id, CancellationToken ct = default)
    {
        var lesson = await _lessons.FindAsync(new object[] { id }, ct).ConfigureAwait(false);

        await base.DeleteByIdAsync(id, ct).ConfigureAwait(false);

        if (lesson != null)
        {
            await CleanCacheForLessonAsync(lesson, ct).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task UpdateBatchAsync(IReadOnlyCollection<Lesson> lessons, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(lessons, nameof(lessons));

        using var transaction = await _context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        try
        {
            _lessons.UpdateRange(lessons);
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            await transaction.CommitAsync(ct).ConfigureAwait(false);

            var cacheTasks = lessons.Select(lesson => CleanCacheForLessonAsync(lesson, ct));
            await Task.WhenAll(cacheTasks).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(ct).ConfigureAwait(false);
            throw;
        }
    }

    #endregion

    #region Приватные помощники (Helpers)

    private async Task<List<Lesson>> GetByGroupFromDb(int groupId, CancellationToken ct)
    {
        return await _lessons
            .AsNoTracking()
            .Include(l => l.Cabinet).ThenInclude(c => c!.Building)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.Subject)
            .Include(l => l.TimeSlot)
            .Include(l => l.WeekType)
            .Where(l => l.GroupId == groupId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    private async Task<List<Lesson>> GetByTeacherFromDb(int teacherId, CancellationToken ct)
    {
        return await _lessons
            .AsNoTracking()
            .Include(l => l.Cabinet).ThenInclude(c => c!.Building)
            .Include(l => l.Teacher)
            .Include(l => l.Group)
            .Include(l => l.Subject)
            .Include(l => l.TimeSlot)
            .Include(l => l.WeekType)
            .Where(l => l.TeacherId == teacherId)
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    private async Task CleanCacheForLessonAsync(Lesson lesson, CancellationToken ct)
    {
        if (!UseCache) return;

        var tasks = new List<Task>();

        if (lesson.GroupId.HasValue)
        {
            tasks.Add(_cache.RemoveAsync($"lessons:group:{lesson.GroupId.Value}", ct));
        }
        else if (lesson.Group != null)
        {
            tasks.Add(_cache.RemoveAsync($"lessons:group:{lesson.Group.Id}", ct));
        }

        if (lesson.TeacherId.HasValue)
        {
            tasks.Add(_cache.RemoveAsync($"lessons:teacher:{lesson.TeacherId.Value}", ct));
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    #endregion
}