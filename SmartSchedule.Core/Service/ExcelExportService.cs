using ClosedXML.Excel;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Infrastructure.Services;

/// <summary>
/// Реализация сервиса экспорта расписания в Excel на основе динамических временных слотов.
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ExcelExportService"/>.
    /// </summary>
    public ExcelExportService(
        ILessonRepository lessonRepository,
        IGroupRepository groupRepository,
        ITimeSlotRepository timeSlotRepository)
    {
        _lessonRepository = lessonRepository ?? throw new ArgumentNullException(nameof(lessonRepository));
        _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
        _timeSlotRepository = timeSlotRepository ?? throw new ArgumentNullException(nameof(timeSlotRepository));
    }

    /// <inheritdoc/>
    public async Task<(byte[] FileContent, string GroupName)> GenerateGroupScheduleAsync(int groupId, CancellationToken ct = default)
    {
        var group = await _groupRepository.GetByIdAsync(groupId, ct).ConfigureAwait(false);
        string groupName = group?.Name ?? "Группа";

        var lessons = await _lessonRepository.GetByGroupIdAsync(groupId, ct).ConfigureAwait(false);

        var allSlots = await _timeSlotRepository.GetAllAsync(ct).ConfigureAwait(false);
        var timeSlots = allSlots.OrderBy(t => t.SlotNumber).ToList();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Расписание");

            for (int i = 0; i < timeSlots.Count; i++)
            {
                var slot = timeSlots[i];
                var cell = worksheet.Cell(i + 2, 1);

                cell.Value = $"{slot.SlotNumber} пара\n({slot.StartTime:HH:mm} - {slot.EndTime:HH:mm})";

                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            var days = new[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            for (int i = 0; i < days.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 2);
                cell.Value = days[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1e293b");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            var lessonsGrid = lessons
                .GroupBy(l => new { l.DayOfWeekId, l.TimeSlotId })
                .ToDictionary(g => g.Key, g => g.ToList());

            var skipCells = new HashSet<(int Row, int Col)>();

            for (int col = 0; col < days.Length; col++)
            {
                int dayId = col + 1;
                for (int row = 0; row < timeSlots.Count; row++)
                {
                    if (skipCells.Contains((row, col))) continue;

                    var slotId = (int?)timeSlots[row].Id;
                    var key = new { DayOfWeekId = dayId, TimeSlotId = slotId };

                    if (!lessonsGrid.TryGetValue(key, out var currentLessons)) continue;

                    string cellText = GetCellText(currentLessons);
                    int mergeCount = 0;

                    for (int nextRow = row + 1; nextRow < timeSlots.Count; nextRow++)
                    {
                        var nextKey = new { DayOfWeekId = dayId, TimeSlotId = (int?)timeSlots[nextRow].Id };
                        if (!lessonsGrid.TryGetValue(nextKey, out var nextLessons)) break;

                        string nextText = GetCellText(nextLessons);

                        if (nextText == cellText)
                        {
                            mergeCount++;
                            skipCells.Add((nextRow, col));
                        }
                        else
                        {
                            break;
                        }
                    }

                    var cell = worksheet.Cell(row + 2, col + 2);
                    cell.Value = cellText;
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if (mergeCount > 0)
                    {
                        worksheet.Range(row + 2, col + 2, row + 2 + mergeCount, col + 2).Merge();
                    }
                }
            }

            var fullRange = worksheet.Range(1, 1, timeSlots.Count + 1, days.Length + 1);
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns(1, 1).Width = 18;
            worksheet.Columns(2, 7).Width = 35;
            worksheet.Rows().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return (stream.ToArray(), groupName);
            }
        }
    }

    private static string GetCellText(List<Lesson> list)
    {
        if (list == null || list.Count == 0) return string.Empty;

        var everyWeekLessons = list.Where(l => l.WeekTypeId != 1 && l.WeekTypeId != 2).ToList();

        if (everyWeekLessons.Count > 0)
        {
            return FormatLessonGroup(everyWeekLessons);
        }

        var numLessons = list.Where(l => l.WeekTypeId == 1).ToList();
        var denLessons = list.Where(l => l.WeekTypeId == 2).ToList();

        if (numLessons.Count > 0 || denLessons.Count > 0)
        {
            string top = numLessons.Count > 0 ? FormatLessonGroup(numLessons) : " ";
            string bottom = denLessons.Count > 0 ? FormatLessonGroup(denLessons) : " ";
            return $"{top}\n--------------------------\n{bottom}";
        }

        return FormatLessonGroup(list);
    }

    private static string FormatLessonGroup(List<Lesson> lessons)
    {
        if (lessons == null || lessons.Count == 0) return string.Empty;

        var subjects = string.Join(", ", lessons.Select(l => l.Subject?.Title).Distinct());
        var teachers = string.Join(", ", lessons.Select(l => l.Teacher?.LastName).Where(n => !string.IsNullOrEmpty(n)).Distinct());
        var cabinets = string.Join(", ", lessons.Select(l => $"Каб. {l.Cabinet?.Number}").Distinct());

        return $"{subjects}\n{teachers}\n{cabinets}";
    }
}