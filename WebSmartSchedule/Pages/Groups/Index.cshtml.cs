using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Application.DTOs.Building;
using SmartSchedule.Core.Models.DTO.GroupDTO;
using SmartSchedule.Core.Models.DTOs.Building;
using System.Net;
using System.Text.Json;

namespace WebSmartSchedule.Pages.Groups
{
    [Authorize(Roles = "Admin, Dispatcher")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ResponseGroupDto> Groups { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            // групппы
            var response = await _httpClient.GetAsync("api/Group/all", ct);
            if (response.IsSuccessStatusCode)
                Groups = await response.Content.ReadFromJsonAsync<List<ResponseGroupDto>>(ct) ?? new();

        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateBuildingDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Group", dto, ct);

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
                var response = await _httpClient.PutAsJsonAsync($"api/Group/{id}", dto, ct);

                if (response.IsSuccessStatusCode)
                {
                    TempData["GRSuccessMessage"] = "Здание успешно обновлено!";
                    return RedirectToPage();
                }

                await HandleErrorResponse(response, "обновлении");
            }
            catch (Exception ex)
            {
                TempData["GRErrorMessage"] = $"Ошибка: {ex.Message}";
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

                TempData["GRErrorMessage"] = problemDetails?.Detail
                    ?? $"Ошибка уникальности при {actionName} здания";
            }
            else
            {
                
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["GRErrorMessage"] = $"Ошибка при {actionName} здания: {response.StatusCode}";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Group/{id}", ct);
                if (response.IsSuccessStatusCode)
                    TempData["GRSuccessMessage"] = "Группа успешно удалена!";
                else
                    TempData["GRErrorMessage"] = "Ошибка при удалении группы.";
            }
            catch (Exception ex)
            {
                TempData["GRErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}