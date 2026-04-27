using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Infrastructure.Data;

namespace SmartSchedule.Infrastructure.Repositories;

/// <summary>
/// Репозиторий для работы с преподавателями.
/// </summary>
public class TeacherRepository : BaseRepository<Teacher, int, AppDbContext>, ITeacherRepository
{
    private readonly DbSet<Teacher> _teachers;

    /// <summary>
    /// Инициализирует новый экземпляр репозитория.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public TeacherRepository(AppDbContext context) : base(context)
    {
        _teachers = context.Teachers;
    }

    /// <summary>
    /// Асинхронно получает список всех преподавателей.
    /// </summary>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список всех преподавателей.</returns>
    public async Task<List<Teacher>> GetAllAsync(CancellationToken ct)
    {
        return await _teachers
            .ToListAsync(ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Выполняет поиск преподавателей по ФИО.
    /// </summary>
    /// <param name="search">Строка поиска.</param>
    /// <returns>Список отфильтрованных преподавателей.</returns>
    public async Task<List<Teacher>> SearchAsync(string? search)
    {
        var query = _teachers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchPattern = $"%{search.Trim()}%";

            query = query.Where(t =>
                EF.Functions.Like(t.LastName + " " + t.FirstName + " " + t.MiddleName, searchPattern));
        }

        return await query
            .ToListAsync()
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Получает данные преподавателя со всеми связанными уроками, группами и таймслотами.
    /// </summary>
    /// <param name="id">Идентификатор преподавателя.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Сущность преподавателя с коллекцией уроков.</returns>
    public async Task<Teacher?> GetWithLessonsByIdAsync(int id, CancellationToken ct)
    {
        return await _teachers
            .Include(t => t.Lessons)
                .ThenInclude(l => l.Group)
            .Include(t => t.Lessons)
                .ThenInclude(l => l.TimeSlot)
            .FirstOrDefaultAsync(t => t.Id == id, ct)
            .ConfigureAwait(false);
    }
}