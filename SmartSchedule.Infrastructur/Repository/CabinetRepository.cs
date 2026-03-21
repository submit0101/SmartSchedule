
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с Кабинетами.
    /// </summary>
    public class CabinetRepository : BaseRepository<Cabinet, int, AppDbContext>, ICabinetRepository
    {
        #region Поля

        private readonly DbSet<Cabinet> _cabinet;

        #endregion

        /// <summary>
        /// Конструктор репозитория Cabinet.
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public CabinetRepository(AppDbContext context) : base(context)
        {
            _cabinet = context.Cabinets;
        }

        /// <summary>
        /// Асинхронно получает все кабинеты, включая связанные строения.
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список кабинетов с загруженными строениями</returns>
        public async Task<List<Cabinet>> GetAllWithBuldingAsync(CancellationToken ct)
        {
            return await _cabinet.Include(c => c.Building).ToListAsync(ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Выполняет поиск кабинетов по номеру и/или зданию с возможностью сортировки.
        /// </summary>
        /// <param name="searchTerm">Числовой номер кабинета (например "310")</param>
        /// <param name="buildingNumber">Номер здания (1 или 2)</param>
        /// <param name="sortBy">Поле для сортировки ("number", "building")</param>
        /// <param name="descending">Направление сортировки (true - по убыванию)</param>
        /// <returns>Список кабинетов, удовлетворяющих условиям</returns>
        public async Task<List<Cabinet>> SearchAsync(
            string? searchTerm,
            int? buildingNumber,
            string? sortBy,
            bool descending)
        {
            IQueryable<Cabinet> query = _cabinet.Include(b=>b.Building);

            // Фильтрация по номеру кабинета
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => EF.Functions.Like(c.Number, $"%{searchTerm}%"));
            }

            // Фильтрация по зданию
            if (buildingNumber.HasValue)
            {
                query = query.Where(c => c.BuildingId == buildingNumber.Value);
            }

            // Сортировка
            IOrderedQueryable<Cabinet> orderedQuery = sortBy?.ToLower() switch
            {
                "building" => descending
                    ? query.OrderByDescending(c => c.BuildingId)
                    : query.OrderBy(c => c.BuildingId),

                _ => descending
                    ? query.OrderByDescending(c => c.Number)
                    : query.OrderBy(c => c.Number)
            };
            ArgumentNullException.ThrowIfNull(orderedQuery);
            return await orderedQuery.ToListAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// фильтер кабинетов
        /// </summary>
        /// <param name="buildingId">здание</param>
        /// <param name="ct">токен отмены</param>
        /// <returns></returns>
        public async Task<List<Cabinet>> GetAllFilteredAsync(int? buildingId, CancellationToken ct)
        {
            var query = _cabinet.AsQueryable();

            if (buildingId.HasValue)
            {
                query = query.Where(c => c.BuildingId == buildingId.Value);
            }

            return await query.Include(c => c.Building).ToListAsync(ct).ConfigureAwait(false);
        }
        /// <summary>
        /// получить кабинеты с расписанием
        /// </summary>
        /// <param name="cabinetId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Cabinet> GetWithLessonsByIdAsync(int cabinetId, CancellationToken ct)
        {
            
            return await _cabinet
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.TimeSlot) 
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.WeekDay) 
                .FirstOrDefaultAsync(c => c.Id == cabinetId, ct).ConfigureAwait(false);
        }
        /// <summary>
        /// Проверка уникальности кабинета
        /// </summary>
        /// <param name="number">номер кабинета</param>
        /// <param name="buildingId">номер корпуса</param>
        /// <param name="excludeId">уникальный иднтификатор записи</param>
        /// <param name="ct">токен отмены</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string number, int buildingId, int? excludeId = null, CancellationToken ct = default)
        {
            return await _cabinet
                .AnyAsync(c => c.Number == number
                            && c.BuildingId == buildingId
                            && (excludeId == null || c.Id != excludeId), ct).ConfigureAwait(false);
        }
    }
}