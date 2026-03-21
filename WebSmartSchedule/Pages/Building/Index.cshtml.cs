using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Application.DTOs.Building;
using SmartSchedule.Core.Models.DTOs.Building;
using System.Net;
using System.Text.Json;

namespace WebSmartSchedule.Pages.Building
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ResponseBuildingDto> Buildings { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var response = await _httpClient.GetAsync("api/Building/all", ct);
            if (response.IsSuccessStatusCode)
                Buildings = await response.Content.ReadFromJsonAsync<List<ResponseBuildingDto>>(ct) ?? new();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateBuildingDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Building", dto, ct);

                if (response.IsSuccessStatusCode)
                {
                    TempData["BUSuccessMessage"] = "Здание успешно добавлено!";
                    return RedirectToPage();
                }

                await HandleErrorResponse(response, "создании");
            }
            catch (Exception ex)
            {
                TempData["BUErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(
            [FromForm] int id,
            [FromForm] UpdateBuildingDto dto,
            CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Building/{id}", dto, ct);

                if (response.IsSuccessStatusCode)
                {
                    TempData["BUSuccessMessage"] = "Здание успешно обновлено!";
                    return RedirectToPage();
                }

                await HandleErrorResponse(response, "обновлении");
            }
            catch (Exception ex)
            {
                TempData["BUErrorMessage"] = $"Ошибка: {ex.Message}";
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

                TempData["BUErrorMessage"] = problemDetails?.Detail
                    ?? $"Ошибка уникальности при {actionName} здания";
            }
            else
            {
                // Общая ошибка для других статус-кодов
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["BUErrorMessage"] = $"Ошибка при {actionName} здания: {response.StatusCode}";
            }
        }
    

        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Building/{id}", ct);
                if (response.IsSuccessStatusCode)
                    TempData["BUSuccessMessage"] = "Здание успешно удалено!";
                else
                    TempData["BUErrorMessage"] = "Ошибка при удалении здания.";
            }
            catch (Exception ex)
            {
                TempData["BUErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}