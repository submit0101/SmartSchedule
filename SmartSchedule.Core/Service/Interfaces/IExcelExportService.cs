using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSchedule.Core.Service.Interfaces;
using System.Threading.Tasks;

/// <summary>
/// Сервис для экспорта данных расписания в формат Excel (.xlsx).
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Асинхронно генерирует файл Excel с расписанием для указанной учебной группы.
    /// </summary>
    /// <param name="groupId">Уникальный идентификатор группы, для которой формируется расписание.</param>
    /// <param name="ct">токен отмены</param>
    /// <returns>
    /// Кортеж, содержащий:
    /// <list type="bullet">
    /// <item><description><c>FileContent</c> — массив байтов готового Excel-файла.</description></item>
    /// <item><description><c>GroupName</c> — строковое название группы (удобно для формирования имени скачиваемого файла).</description></item>
    /// </list>
    /// </returns>
    Task<(byte[] FileContent, string GroupName)> GenerateGroupScheduleAsync(int groupId, CancellationToken ct = default);
}