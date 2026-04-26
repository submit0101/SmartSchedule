using SmartSchedule.Core.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Repositories;

/// <summary>
/// Интерфейс репозитория для выполнения сложных аналитических и сводных запросов отчетов.
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Получает полный список занятий со всеми связанными сущностями для построения аналитических отчетов.
    /// </summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список занятий со связанными данными.</returns>
    Task<List<Lesson>> GetLessonsForReportAsync(CancellationToken ct = default);

    /// <summary>
    /// Извлекает матрицу занятости (идентификаторы преподавателей, дней и слотов) для заданных преподавателей и типа недели.
    /// Учитывает пары, которые проводятся на обеих неделях.
    /// </summary>
    /// <param name="teacherIds">Коллекция идентификаторов преподавателей (только для чтения).</param>
    /// <param name="weekTypeId">Идентификатор типа недели.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список кортежей, представляющих занятые временные слоты преподавателей.</returns>
    Task<List<(int TeacherId, int DayId, int SlotId)>> GetBusyMatrixAsync(ReadOnlyCollection<int> teacherIds, int weekTypeId, CancellationToken ct = default);
}