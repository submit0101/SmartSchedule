using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.PosittonDTO;
using SmartSchedule.Core.Models.DTO.TeacherDTO;

namespace WebSmartSchedule.Pages.Teacher;
[Authorize(Roles ="Admin,Dispatcher")]
public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;
    public List<ResponseTeacherDto> Teachers { get; set; } = new();
    public List<ShortPositionDto> Positions { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? PositionId { get; set; }

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task OnGetAsync(CancellationToken ct)
    {
        // Базовый URL для поиска преподавателей
        var searchUrl = "api/Teacher/search";

        var queryParams = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(SearchTerm))
            queryParams.Add("search", SearchTerm);

        if (PositionId.HasValue)
            queryParams.Add("positionId", PositionId.Value.ToString());

        // Формируем URL с параметрами
        if (queryParams.Any())
        {
            searchUrl += "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        }

        // Получаем преподавателей
        var teacherResponse = await _httpClient.GetAsync(searchUrl, ct);
        if (teacherResponse.IsSuccessStatusCode)
        {
            Teachers = await teacherResponse.Content.ReadFromJsonAsync<List<ResponseTeacherDto>>(ct)
                       ?? new List<ResponseTeacherDto>();
        }
        else
        {
            Console.WriteLine($"Ошибка при получении преподавателей: {teacherResponse.StatusCode}");
        }

        // Получаем список должностей для фильтрации
        var positionResponse = await _httpClient.GetAsync("api/Position/Short", ct);
        if (positionResponse.IsSuccessStatusCode)
        {
            Positions = await positionResponse.Content.ReadFromJsonAsync<List<ShortPositionDto>>(ct)
                         ?? new List<ShortPositionDto>();
        }
        else
        {
            Console.WriteLine($"Ошибка при получении должностей: {positionResponse.StatusCode}");
        }
    }

    // === POST: Добавление преподавателя ===
    public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateTeacherDto dto, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Teacher", dto, ct);
            if (!response.IsSuccessStatusCode)
            {
                TempData["TEErrorMessage"] = "Ошибка при добавлении преподавателя.";
            }
            else
            {
                TempData["TESuccessMessage"] = "Преподаватель успешно добавлен!";
            }
        }
        catch (Exception ex)
        {
            TempData["TEErrorMessage"] = $"Ошибка при создании: {ex.Message}";
        }

        return RedirectToPage();
    }

    // === POST: Обновление преподавателя ===
    public async Task<IActionResult> OnPostUpdateAsync(
        [FromForm] int id,
        [FromForm] UpdateTeacherDto dto,
        CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Teacher/{id}", dto, ct);

            if (response.IsSuccessStatusCode)
            {
                TempData["TESuccessMessage"] = "Преподаватель успешно обновлён!";
            }
            else
            {
                TempData["TEErrorMessage"] = "Ошибка при обновлении преподавателя.";
            }
        }
        catch (Exception ex)
        {
            TempData["TEErrorMessage"] = $"Ошибка при обновлении: {ex.Message}";
        }

        return RedirectToPage();
    }

    // === POST: Удаление преподавателя ===
    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/Teacher/{id}", ct);

            if (response.IsSuccessStatusCode)
            {
                TempData["TESuccessMessage"] = "Преподаватель успешно удалён!";
            }
            else
            {
                TempData["TEErrorMessage"] = "Ошибка при удалении преподавателя.";
            }
        }
        catch (Exception ex)
        {
            TempData["TEErrorMessage"] = $"Ошибка при удалении: {ex.Message}";
        }

        return RedirectToPage();
    }
}