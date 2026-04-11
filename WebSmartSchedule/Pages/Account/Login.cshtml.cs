using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using SmartSchedule.Core.Models.DTO.AuthDTO;

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
            // 2. Ура, API принял логин/пароль! Достаем токен (если он тебе нужен)
            // var result = await response.Content.ReadFromJsonAsync<ТвояМодельОтвета>();

            // 3. Говорим браузеру "Пользователь вошел" (создаем куку)
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, Input.Username) };
            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

            return RedirectToPage("/Index");
        }

        // Если API вернул ошибку (401 Unauthorized и т.д.)
        ModelState.AddModelError(string.Empty, "Неверный логин или пароль (ответил API).");
        return Page();
    }
}