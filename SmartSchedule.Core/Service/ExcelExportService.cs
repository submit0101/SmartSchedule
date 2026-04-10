using ClosedXML.Excel;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;
using System;
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
    /// <param name="lessonRepository">Репозиторий для работы с занятиями.</param>
    /// <param name="groupRepository">Репозиторий для работы с группами.</param>
    /// <param name="timeSlotRepository">Репозиторий для работы с временными слотами.</param>
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
                .ToDictionary(g => g.Key, g => g.OrderBy(l => l.WeekTypeId).ToList());

            var processedLessons = new HashSet<int>();

            foreach (var slotEntry in lessonsGrid)
            {
                var dayId = slotEntry.Key.DayOfWeekId;
                var slotId = slotEntry.Key.TimeSlotId;
                var lessonsInSlot = slotEntry.Value;

                int rowIndex = timeSlots.FindIndex(t => t.Id == slotId) + 2;
                int colIndex = dayId + 1;

                if (rowIndex < 2) continue;

                var currentCell = worksheet.Cell(rowIndex, colIndex);
                var firstLesson = lessonsInSlot.First();

                if (processedLessons.Contains(firstLesson.Id)) continue;

                var nextSlot = timeSlots.ElementAtOrDefault(timeSlots.FindIndex(t => t.Id == slotId) + 1);
                if (nextSlot != null && lessonsGrid.TryGetValue(new { DayOfWeekId = dayId, TimeSlotId = (int?)nextSlot.Id }, out var nextLessons))
                {
                    var nextLesson = nextLessons.FirstOrDefault(nl =>
                        nl.SubjectId == firstLesson.SubjectId &&
                        nl.TeacherId == firstLesson.TeacherId &&
                        nl.WeekTypeId == firstLesson.WeekTypeId);

                    if (nextLesson != null)
                    {
                        worksheet.Range(rowIndex, colIndex, rowIndex + 1, colIndex).Merge();
                        processedLessons.Add(nextLesson.Id);
                    }
                }

                if (lessonsInSlot.Count > 1)
                {
                    string content = string.Join("\n--------------------------\n",
                        lessonsInSlot.Select(l => FormatLessonText(l)));
                    currentCell.Value = content;
                }
                else
                {
                    currentCell.Value = FormatLessonText(firstLesson);
                }

                currentCell.Style.Alignment.WrapText = true;
                currentCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                currentCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            var fullRange = worksheet.Range(1, 1, timeSlots.Count + 1, days.Length + 1);
            fullRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            fullRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns(1, 1).Width = 18;
            worksheet.Columns(2, 7).Width = 30;
            worksheet.Rows().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return (stream.ToArray(), groupName);
            }
        }
    }

    private static string FormatLessonText(Lesson lesson)
    {
        if (lesson == null) return string.Empty;
        return $"{lesson.Subject?.Title}\n{lesson.Teacher?.LastName}\nКаб. {lesson.Cabinet?.Number}";
    }
}