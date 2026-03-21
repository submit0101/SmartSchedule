using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Application.DTOs;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Models.DTO.SubjectDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WebSmartSchedule.Pages.Lessons
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int SelectedGroupId { get; set; } = 0; // По умолчанию 0

        public List<ResponseGroupDto> Groups { get; set; } = new();
        public List<ResponseSubjectDto> Subjects { get; set; } = new();
        public List<ShortTeacherDto> Teachers { get; set; } = new();
        public List<ShortCabinetDto> Cabinets { get; set; } = new();
        public List<ShortBuildingDto> Buildings { get; set; } = new();
        public List<ResponseTimeSlotDto> TimeSlots { get; set; } = new();
        public List<ResponseWeekTypeDto> WeekTypes { get; set; } = new();
        public List<WeekDayShortDto> WeekDays { get; set; } = new();

        public async Task OnGetAsync(CancellationToken ct)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            // Запускаем все запросы параллельно (без await)
            var groupsTask = client.GetAsync("api/group/all", ct);
            var subjectsTask = client.GetAsync("api/subject/all", ct);
            var teachersTask = client.GetAsync("api/teacher/Short", ct);
            var buildingsTask = client.GetAsync("api/Building/Short", ct);
            var cabinetsTask = client.GetAsync("api/cabinet/Short", ct);
            var timeSlotsTask = client.GetAsync("api/timeslot/all", ct);
            var weekTypesTask = client.GetAsync("api/weektype/all", ct);
            var weekDaysTask = client.GetAsync("/api/WeekDay/short", ct);

            // Ждем всех
            await Task.WhenAll(
                groupsTask, subjectsTask, teachersTask, buildingsTask,
                cabinetsTask, timeSlotsTask, weekTypesTask, weekDaysTask
            );

            // Распаковка результатов
            var groupsResp = groupsTask.Result;
            if (groupsResp.IsSuccessStatusCode)
                Groups = await groupsResp.Content.ReadFromJsonAsync<List<ResponseGroupDto>>(ct) ?? new();

            var subjectsResp = subjectsTask.Result;
            if (subjectsResp.IsSuccessStatusCode)
                Subjects = await subjectsResp.Content.ReadFromJsonAsync<List<ResponseSubjectDto>>(ct) ?? new();

            var teachersResp = teachersTask.Result;
            if (teachersResp.IsSuccessStatusCode)
                Teachers = await teachersResp.Content.ReadFromJsonAsync<List<ShortTeacherDto>>(ct) ?? new();

            var buildingsResp = buildingsTask.Result;
            if (buildingsResp.IsSuccessStatusCode)
                Buildings = await buildingsResp.Content.ReadFromJsonAsync<List<ShortBuildingDto>>(ct) ?? new();

            var cabinetsResp = cabinetsTask.Result;
            if (cabinetsResp.IsSuccessStatusCode)
                Cabinets = await cabinetsResp.Content.ReadFromJsonAsync<List<ShortCabinetDto>>(ct) ?? new();

            var timeSlotsResp = timeSlotsTask.Result;
            if (timeSlotsResp.IsSuccessStatusCode)
                TimeSlots = await timeSlotsResp.Content.ReadFromJsonAsync<List<ResponseTimeSlotDto>>(ct) ?? new();

            var weekTypesResp = weekTypesTask.Result;
            if (weekTypesResp.IsSuccessStatusCode)
                WeekTypes = await weekTypesResp.Content.ReadFromJsonAsync<List<ResponseWeekTypeDto>>(ct) ?? new();

            var weekDaysResp = weekDaysTask.Result;
            if (weekDaysResp.IsSuccessStatusCode)
                WeekDays = await weekDaysResp.Content.ReadFromJsonAsync<List<WeekDayShortDto>>(ct) ?? new();

            // --- ИСПРАВЛЕНИЕ: Умный выбор группы по умолчанию ---
            if (Groups.Any())
            {
                // Если ID не пришел (0) ИЛИ такой группы больше нет в списке
                if (SelectedGroupId == 0 || !Groups.Any(g => g.Id == SelectedGroupId))
                {
                    // Берем ПЕРВУЮ РЕАЛЬНУЮ группу из базы
                    SelectedGroupId = Groups.OrderBy(g => g.Name).First().Id;
                }
            }
            // Если групп нет вообще - SelectedGroupId останется 0, и фронтенд просто покажет пустую сетку.
            // Никаких фейковых "Группа 1".
        }

        public async Task<IActionResult> OnPostCreateLessonAsync(CreateLessonDto dto, CancellationToken ct)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            try
            {
                var response = await client.PostAsJsonAsync("api/lesson", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Занятие успешно добавлено!";
                    TempData["AlertType"] = "success";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(ct);
                    TempData["AlertMessage"] = $"Ошибка: {response.StatusCode} - {errorContent}";
                    TempData["AlertType"] = "danger";
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = $"Ошибка: {ex.Message}";
                TempData["AlertType"] = "danger";
            }
            return RedirectToPage(new { SelectedGroupId = dto.GroupId });
        }
    }
}