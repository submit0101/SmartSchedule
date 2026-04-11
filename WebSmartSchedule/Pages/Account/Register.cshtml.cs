using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.AuthDTO;
using System.Security.Claims;

namespace WebSmartSchedule.Pages.Account;

/// <summary>
/// Модель страницы регистрации, работающая через API.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly HttpClient _httpClient;

    public RegisterModel(IHttpClientFactory httpClientFactory)
    {
        // Используем тот же именованный клиент, что и в логине
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    [BindProperty]
    public RegisterDto Input { get; set; } = null!;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // 1. Отправляем запрос на регистрацию в API
        // Убедись, что путь "api/Auth/register" совпадает с твоим контроллером в API
        var response = await _httpClient.PostAsJsonAsync("api/Auth/register", Input);

        if (response.IsSuccessStatusCode)
        {
            // 2. Регистрация прошла успешно! 
            // Теперь нам нужно авторизовать пользователя в браузере (создать куку),
            // чтобы ему не пришлось вводить пароль сразу после регистрации.

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Username),
                new Claim(ClaimTypes.Email, Input.Email)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Создаем ту самую легкую куку, которую ждет наш _Layout и Index
            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

            return RedirectToPage("/Index");
        }

        // 3. Если API вернул ошибку (например, такой логин уже занят)
        // Можно попробовать прочитать текст ошибки от API
        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, $"Ошибка регистрации: {errorContent}");

        return Page();
    }
}