
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Infrastructure.Data;

namespace SmartSchedule.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с Преподавателями
    /// </summary>
    public class TeacherRepository : BaseRepository<Teacher, int, AppDbContext>, ITeacherRepository
    {
        #region Поля

        private readonly DbSet<Teacher> _teatcher;

        #endregion
        /// <summary>
        /// Конструктор репозитория Teacher
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public TeacherRepository(AppDbContext context) : base(context)
        {
            _teatcher = context.Teachers;
        }
        /// <summary>
        /// Ансихрнонно получает всех учителй включая связанные позиции
        /// </summary>
        /// <param name="ct">токен отмены</param>
        /// <returns></returns>
        public async Task<List<Teacher>> GetAllWithPositonAsync(CancellationToken ct)
        {
            return await _teatcher.Include(c=> c.Position).ToListAsync(ct).ConfigureAwait(false);

        }
        /// <summary>
        /// Выполняет поиск преподавателей по ФИО и фильтрацию по должности.
        /// </summary>
        /// <param name="search">Часть ФИО для поиска.</param>
        /// <param name="positionValue">Идентификатор должности.</param>
        /// <returns>Список найденных преподавателей.</returns>
        public async Task<List<Teacher>> SearchAsync(string? search, int? positionValue)
        {
            var query = _teatcher.Include(t => t.Position).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Убираем лишние пробелы и добавляем символы подстановки
                var searchPattern = $"%{search.Trim()}%";

                query = query.Where(t =>
                    EF.Functions.Like(t.LastName + " " + t.FirstName + " " + t.MiddleName, searchPattern));
            }

            if (positionValue.HasValue)
            {
                query = query.Where(t => t.PositionId == positionValue.Value);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Получает преподавателя по идентификатору с присоединенными (Eagerly Loaded) связанными занятиями.
        /// </summary>
        /// <param name="id">Уникальный идентификатор преподавателя.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>
        /// Объект сущности <see cref="Teacher"/> с заполненной коллекцией <see cref="Teacher.Lessons"/> 
        /// (включая <see cref="Group"/> и <see cref="TimeSlot"/>), 
        /// или <see langword="null"/>, если преподаватель не найден.
        /// </returns>
        public async Task<Teacher?> GetWithLessonsByIdAsync(int id, CancellationToken ct) 
        {
            var teacher = await _teatcher
                .Include(t => t.Lessons)
                    .ThenInclude(l => l.Group)
                .Include(t => t.Lessons)
                    .ThenInclude(l => l.TimeSlot)
                .FirstOrDefaultAsync(t => t.Id == id, ct) 
                .ConfigureAwait(false);
            return teacher;
        }

    }
}