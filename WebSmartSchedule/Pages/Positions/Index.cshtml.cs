using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.PosittonDTO;
using System.Net;
using System.Text.Json;

namespace WebSmartSchedule.Pages.Positions
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ResponsePositionDto> Positions { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var response = await _httpClient.GetAsync("api/Position/all", ct);
            if (response.IsSuccessStatusCode)
                Positions = await response.Content.ReadFromJsonAsync<List<ResponsePositionDto>>(ct) ?? new();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreatePositionDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Position", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["POSuccessMessage"] = "Должность успешно добавлена!";
                    return RedirectToPage();
                }
                await HandleErrorResponse(response, "создания");
            }
            catch (Exception ex)
            {
                TempData["POErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(
            [FromForm] int id,
            [FromForm] UpdatePositionDto dto,
            CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Position/{id}", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["POSuccessMessage"] = "Должность успешно обновлена!";
                    return RedirectToPage();
                }
                await HandleErrorResponse(response, "обновлении");
            }
            catch (Exception ex)
            {
                TempData["POErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }

        private async Task HandleErrorResponse(HttpResponseMessage response, string actionName)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                // Десериализация ответа как ProblemDetails
                var content = await response.Content.ReadAsStringAsync();
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                TempData["POErrorMessage"] = problemDetails?.Detail
                    ?? $"Ошибка уникальности при {actionName} Должность";
            }
            else
            {

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["POErrorMessage"] = $"Ошибка при {actionName} Должности: {response.StatusCode}";
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Position/{id}", ct);
                if (response.IsSuccessStatusCode)
                    TempData["POSuccessMessage"] = "Должность успешно удалена!";
                else
                    TempData["POErrorMessage"] = "Ошибка при удалении должности.";
            }
            catch (Exception ex)
            {
                TempData["POErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}