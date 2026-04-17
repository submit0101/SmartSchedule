using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebSmartSchedule.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    // Список юзеров для вывода в HTML
    public List<UserDto> Users { get; set; } = new();

    // --- СВОЙСТВА ДЛЯ ПРИВЯЗКИ ДАННЫХ ИЗ ФОРМ (МОДАЛОК) ---
    [BindProperty]
    public AssignRoleDto RoleAssignInput { get; set; } = new();

    [BindProperty]
    public CreateUserDto CreateInput { get; set; } = new();

    [BindProperty]
    public UpdateUserDto UpdateInput { get; set; } = new();

    // ================== ЗАГРУЗКА СТРАНИЦЫ ==================
    public async Task<IActionResult> OnGetAsync()
    {
        SetAuthorizationHeader();

        // Получаем список юзеров из API
        Users = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Auth/users") ?? new List<UserDto>();
        return Page();
    }

    // ================== 1. СМЕНА РОЛИ ==================
    public async Task<IActionResult> OnPostAssignRoleAsync()
    {
        SetAuthorizationHeader();

        var response = await _httpClient.PostAsJsonAsync("api/Auth/assign-role", RoleAssignInput);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Роль успешно изменена!";
        else
            TempData["ErrorMessage"] = "Ошибка при изменении роли.";

        return RedirectToPage();
    }

    // ================== 2. СОЗДАНИЕ ПОЛЬЗОВАТЕЛЯ ==================
    public async Task<IActionResult> OnPostCreateUserAsync()
    {
        SetAuthorizationHeader();

        var response = await _httpClient.PostAsJsonAsync("api/Auth/create", CreateInput);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = $"Пользователь {CreateInput.Username} успешно создан!";
        else
            TempData["ErrorMessage"] = "Ошибка при создании пользователя. Возможно, логин уже занят.";

        return RedirectToPage();
    }

    // ================== 3. РЕДАКТИРОВАНИЕ ПОЛЬЗОВАТЕЛЯ ==================
    public async Task<IActionResult> OnPostEditUserAsync()
    {
        SetAuthorizationHeader();

        // Используем PUT запрос для обновления
        var response = await _httpClient.PutAsJsonAsync("api/Auth/update", UpdateInput);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Данные пользователя успешно обновлены!";
        else
            TempData["ErrorMessage"] = "Ошибка при обновлении пользователя.";

        return RedirectToPage();
    }

    // --- Вспомогательный метод, чтобы не дублировать код ---
    private void SetAuthorizationHeader()
    {
        var token = User.FindFirst("AccessToken")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}