using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;

namespace WebSmartSchedule.Pages.TimeSlots
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ResponseTimeSlotDto> TimeSlots { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var response = await _httpClient.GetAsync("api/TimeSlot/all", ct);
            if (response.IsSuccessStatusCode)
            {
                TimeSlots = await response.Content.ReadFromJsonAsync<List<ResponseTimeSlotDto>>(ct)
                           ?? new List<ResponseTimeSlotDto>();
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateTimeSlotDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/TimeSlot", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["TSSuccessMessage"] = "Временной слот успешно добавлен!";
                }
                else
                {
                    TempData["TSErrorMessage"] = "Ошибка при создании временного слота.";
                }
            }
            catch (Exception ex)
            {
                TempData["TSErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(
            [FromForm] int id,
            [FromForm] UpdateTimeSlotDto dto,
            CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/TimeSlot/{id}", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["TSSuccessMessage"] = "Временной слот успешно обновлён!";
                }
                else
                {
                    TempData["TSErrorMessage"] = "Ошибка при обновлении временного слота.";
                }
            }
            catch (Exception ex)
            {
                TempData["TSErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/TimeSlot/{id}", ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["TSSuccessMessage"] = "Временной слот успешно удалён!";
                }
                else
                {
                    TempData["TSErrorMessage"] = "Ошибка при удалении временного слота.";
                }
            }
            catch (Exception ex)
            {
                TempData["TSErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}