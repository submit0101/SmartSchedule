using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.TeacherDTO;

namespace WebSmartSchedule.Pages.Teacher;

/// <summary>
/// Модель страницы управления преподавателями.
/// </summary>
public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Список преподавателей для отображения.
    /// </summary>
    public List<ResponseTeacherDto> Teachers { get; set; } = new();

    /// <summary>
    /// Поисковый запрос по ФИО.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    /// <summary>
    /// Загрузка данных страницы.
    /// </summary>
    public async Task OnGetAsync(CancellationToken ct)
    {
        // Базовый URL для поиска
        var searchUrl = "api/Teacher/search";

        // Формируем параметры запроса
        if (!string.IsNullOrEmpty(SearchTerm))
        {
            searchUrl += $"?search={Uri.EscapeDataString(SearchTerm)}";
        }

        // Получаем преподавателей через API
        var teacherResponse = await _httpClient.GetAsync(searchUrl, ct);

        if (teacherResponse.IsSuccessStatusCode)
        {
            Teachers = await teacherResponse.Content.ReadFromJsonAsync<List<ResponseTeacherDto>>(ct)
                       ?? new List<ResponseTeacherDto>();
        }
        else
        {
            // Здесь можно добавить логгирование ошибки
            Teachers = new List<ResponseTeacherDto>();
        }
    }

    /// <summary>
    /// Обработка создания нового преподавателя.
    /// </summary>
    public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateTeacherDto dto, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Teacher", dto, ct);
            if (response.IsSuccessStatusCode)
            {
                TempData["TESuccessMessage"] = "Преподаватель успешно добавлен!";
            }
            else
            {
                TempData["TEErrorMessage"] = "Ошибка при добавлении преподавателя.";
            }
        }
        catch (Exception ex)
        {
            TempData["TEErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage();
    }

    /// <summary>
    /// Обработка обновления данных преподавателя.
    /// </summary>
    public async Task<IActionResult> OnPostUpdateAsync(int id, [FromForm] UpdateTeacherDto dto, CancellationToken ct)
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
            TempData["TEErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage();
    }

    /// <summary>
    /// Обработка удаления преподавателя.
    /// </summary>
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
                TempData["TEErrorMessage"] = "Ошибка при удалении.";
            }
        }
        catch (Exception ex)
        {
            TempData["TEErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage();
    }
}