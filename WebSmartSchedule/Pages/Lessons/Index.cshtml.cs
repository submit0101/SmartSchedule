using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using SmartSchedule.Application.DTOs;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTO.CabinetDTO;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Models.DTO.SubjectDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;


namespace WebSmartSchedule.Pages.Lessons;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [BindProperty(SupportsGet = true)]
    public int SelectedGroupId { get; set; } = 0;

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

        var groupsTask = client.GetAsync("api/group/all", ct);
        var subjectsTask = client.GetAsync("api/subject/all", ct);
        var teachersTask = client.GetAsync("api/teacher/Short", ct);
        var buildingsTask = client.GetAsync("api/Building/Short", ct);
        var cabinetsTask = client.GetAsync("api/cabinet/Short", ct);
        var timeSlotsTask = client.GetAsync("api/timeslot/all", ct);
        var weekTypesTask = client.GetAsync("api/weektype/all", ct);
        var weekDaysTask = client.GetAsync("/api/WeekDay/short", ct);

        await Task.WhenAll(
            groupsTask, subjectsTask, teachersTask, buildingsTask,
            cabinetsTask, timeSlotsTask, weekTypesTask, weekDaysTask
        );

        if (groupsTask.Result.IsSuccessStatusCode)
            Groups = await groupsTask.Result.Content.ReadFromJsonAsync<List<ResponseGroupDto>>(ct) ?? new();

        if (subjectsTask.Result.IsSuccessStatusCode)
            Subjects = await subjectsTask.Result.Content.ReadFromJsonAsync<List<ResponseSubjectDto>>(ct) ?? new();

        if (teachersTask.Result.IsSuccessStatusCode)
            Teachers = await teachersTask.Result.Content.ReadFromJsonAsync<List<ShortTeacherDto>>(ct) ?? new();

        if (buildingsTask.Result.IsSuccessStatusCode)
            Buildings = await buildingsTask.Result.Content.ReadFromJsonAsync<List<ShortBuildingDto>>(ct) ?? new();

        if (cabinetsTask.Result.IsSuccessStatusCode)
            Cabinets = await cabinetsTask.Result.Content.ReadFromJsonAsync<List<ShortCabinetDto>>(ct) ?? new();

        if (timeSlotsTask.Result.IsSuccessStatusCode)
            TimeSlots = await timeSlotsTask.Result.Content.ReadFromJsonAsync<List<ResponseTimeSlotDto>>(ct) ?? new();

        if (weekTypesTask.Result.IsSuccessStatusCode)
            WeekTypes = await weekTypesTask.Result.Content.ReadFromJsonAsync<List<ResponseWeekTypeDto>>(ct) ?? new();

        if (weekDaysTask.Result.IsSuccessStatusCode)
            WeekDays = await weekDaysTask.Result.Content.ReadFromJsonAsync<List<WeekDayShortDto>>(ct) ?? new();

        if (Groups.Any() && (SelectedGroupId == 0 || !Groups.Any(g => g.Id == SelectedGroupId)))
        {
            SelectedGroupId = Groups.OrderBy(g => g.Name).First().Id;
        }
    }

    public async Task<IActionResult> OnPostCreateLessonAsync(CreateLessonDto dto, CancellationToken ct)
    {
        if (!User.IsInRole("Admin") && !User.IsInRole("Dispatcher"))
        {
            TempData["AlertMessage"] = "У вас нет прав для добавления занятий!";
            TempData["AlertType"] = "danger";
            return RedirectToPage(new { SelectedGroupId = dto.GroupId });
        }

        var client = _clientFactory.CreateClient("ApiClient");

        var token = User.FindFirst("AccessToken")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

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
            TempData["AlertMessage"] = $"Ошибка связи с сервером: {ex.Message}";
            TempData["AlertType"] = "danger";
        }

        return RedirectToPage(new { SelectedGroupId = dto.GroupId });
    }
}