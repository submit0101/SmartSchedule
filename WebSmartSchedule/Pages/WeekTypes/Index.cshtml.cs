using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;

namespace WebSmartSchedule.Pages.WeekTypes
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ResponseWeekTypeDto> WeekTypes { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var response = await _httpClient.GetAsync("api/WeekType/all", ct);
            if (response.IsSuccessStatusCode)
                WeekTypes = await response.Content.ReadFromJsonAsync<List<ResponseWeekTypeDto>>(ct) ?? new();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateWeekTypeDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/WeekType", dto, ct);
                if (response.IsSuccessStatusCode)
                    TempData["WTSuccessMessage"] = "Тип недели успешно добавлен!";
                else
                    TempData["WTErrorMessage"] = "Ошибка при создании типа недели.";
            }
            catch (Exception ex)
            {
                TempData["WTErrorMessage"] = $"Ошибка: {ex.Message}";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(
            [FromForm] int id,
            [FromForm] UpdateWeekTypeDto dto,
            CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/WeekType/{id}", dto, ct);
                if (response.IsSuccessStatusCode)
                    TempData["WTSuccessMessage"] = "Тип недели успешно обновлен!";
                else
                    TempData["WTErrorMessage"] = "Ошибка при обновлении типа недели.";
            }
            catch (Exception ex)
            {
                TempData["WTErrorMessage"] = $"Ошибка: {ex.Message}";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/WeekType/{id}", ct);
                if (response.IsSuccessStatusCode)
                    TempData["WTSuccessMessage"] = "Тип недели успешно удален!";
                else
                    TempData["WTErrorMessage"] = "Ошибка при удалении типа недели.";
            }
            catch (Exception ex)
            {
                TempData["WTErrorMessage"] = $"Ошибка: {ex.Message}";
            }
            return RedirectToPage();
        }
    }
}