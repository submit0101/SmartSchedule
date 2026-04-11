using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebSmartSchedule.Pages.Account;

public class LogoutModel : PageModel
{
    // Ётот метод срабатывает, когда мы нажимаем красную кнопку "¬ыйти"
    public async Task<IActionResult> OnPostAsync()
    {
        // 1. ѕринудительно удал€ем куку авторизации браузера
        await HttpContext.SignOutAsync("MyCookieAuth");

        // 2. ѕеренаправл€ем пользовател€ на главную страницу (где его встретит экран дл€ гостей)
        return RedirectToPage("/Index");
    }
}