using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebSmartSchedule.Pages.Account;

public class LoginModel : PageModel
{
    private readonly HttpClient _httpClient;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        // Берем клиент, который настроили в Program.cs
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    [BindProperty]
    public LoginDto Input { get; set; } = null!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // 1. ПИНАЕМ ЭНДПОИНТ ТВОЕГО API
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", Input);

        if (response.IsSuccessStatusCode)
        {
            // 2. Ура, API принял логин/пароль! Достаем токен и роль из ответа
            var apiResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            if (apiResponse != null)
            {
                // 3. Собираем все данные в список (Имя, Токен, РОЛЬ)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, apiResponse.Username),
                    new Claim("AccessToken", apiResponse.Token), // Сохраняем токен, чтобы потом отправлять его к закрытым методам API
                    new Claim(ClaimTypes.Role, apiResponse.Role ?? "Viewer") // Если роли нет, считаем его просто зрителем
                };

                // 4. Говорим браузеру "Пользователь вошел" (создаем куку)
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                return RedirectToPage("/Index"); // Перекидываем на главную
            }
        }

        // Если API вернул ошибку (401 Unauthorized и т.д.)
        ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
        return Page();
    }
}