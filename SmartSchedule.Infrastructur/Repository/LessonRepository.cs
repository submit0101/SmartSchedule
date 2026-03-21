using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Infrastructure.Data;
using SmartSchedule.Core.Constants;

namespace SmartSchedule.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с сущностями занятий с точечным кэшированием.
/// </summary>
public class LessonRepository : BaseRepository<Lesson, int, AppDbContext>, ILessonRepository
{
    private readonly DbSet<Lesson> _lessons;
    private readonly ICachingService _cache;

    // ИСПРАВЛЕНИЕ 1: Заменили const на static readonly.
    // Это убирает ошибку "Недостижимый код" (CS0162), но сохраняет функционал рубильника.
    private static bool  UseCache = true;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="LessonRepository"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    /// <param name="cache">Сервис кэширования.</param>
    public LessonRepository(AppDbContext context, ICachingService cache) : base(context)
    {
        ArgumentNullException.ThrowIfNull(cache, nameof(cache));
        _lessons = context.Set<Lesson>();
        _cache = cache;
    }

    #region Чтение данных (GET)

    /// <inheritdoc />
    public override async Task<List<Lesson>> GetAllAsync(CancellationToken ct = default)
    {
        // ConfigureAwait(false) добавлен для подавления предупреждения CA2007
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

        // ИСПРАВЛЕНИЕ: Безопасное использование Nullable типов
        if (weekTypeId.HasValue && weekTypeId.Value != WeekTypeConstants.Both)
        {
            // Сохраняем значение в локальную переменную для корректной трансляции EF
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

    /// <summary>
    /// Получает уникальные идентификаторы кабинетов, которые заняты в указанный слот.
    /// </summary>
    /// <param name="dayOfWeekId">Идентификатор дня недели.</param>
    /// <param name="timeSlotId">Идентификатор временного слота.</param>
    /// <param name="weekTypeId">Идентификатор типа недели.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Множество занятых ID кабинетов.</returns>
    public async Task<HashSet<int>> GetBusyCabinetIdsAsync(
        int dayOfWeekId,
        int timeSlotId,
        int weekTypeId,
        CancellationToken ct = default)
    {
        const int BothWeeksId = WeekTypeConstants.Both;

        // EF Core корректно транслирует .Value внутри запроса, если есть проверка HasValue/!= null.
        // Но для успокоения компилятора можно использовать явное приведение там, где мы уверены.
        var busyIds = await _lessons
            .AsNoTracking()
            .Where(l => l.DayOfWeekId == dayOfWeekId &&
                        l.TimeSlotId == timeSlotId &&
                        (l.WeekTypeId == weekTypeId || l.WeekTypeId == BothWeeksId) &&
                        l.CabinetId != null) // Проверка на null
            .Select(l => (int)l.CabinetId!) // Явное указание, что тут не null
            .Distinct()
            .ToHashSetAsync(ct)
            .ConfigureAwait(false);

        return busyIds;
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

    #endregion

    #region Приватные помощники (Helpers)

    private async Task<List<Lesson>> GetByGroupFromDb(int groupId, CancellationToken ct)
    {
        return await _lessons
            .AsNoTracking()
            .Include(l => l.Cabinet).ThenInclude(c => c.Building)
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
            .Include(l => l.Cabinet).ThenInclude(c => c.Building)
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
            // Добавили ConfigureAwait(false)
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

        // ИСПРАВЛЕНИЕ: Заменили .Any() на .Count > 0 для оптимизации
        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    #endregion
}