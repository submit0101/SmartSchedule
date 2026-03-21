using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.SubjectDTO;
using System.Net;
using System.Text.Json;

namespace WebSmartSchedule.Pages.Subjects
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<ResponseSubjectDto> Subjects { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
            var response = await _httpClient.GetAsync("api/Subject/all", ct);
            if (response.IsSuccessStatusCode)
                Subjects = await response.Content.ReadFromJsonAsync<List<ResponseSubjectDto>>(ct) ?? new();
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateSubjectDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Subject", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SUSuccessMessage"] = "Предмет успешно добавлен!";
                    return RedirectToPage();
                }
                await HandleErrorResponse(response, "создании");
            }
            catch (Exception ex)
            {
                TempData["SUErrorMessage"] = $"Ошибка: {ex.Message}";
            }
            return RedirectToPage();

        }

        public async Task<IActionResult> OnPostUpdateAsync(
            [FromForm] int id,
            [FromForm] UpdateSubjectDto dto,
            CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Subject/{id}", dto, ct);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SUSuccessMessage"] = "Предмет успешно обновлён!";
                    return RedirectToPage();
                }
                await HandleErrorResponse(response, "ошибка обновления");
            }
            catch (Exception ex)
            {
                TempData["SUErrorMessage"] = $"Ошибка: {ex.Message}";
            }
            return RedirectToPage();

        }
        private async Task HandleErrorResponse(HttpResponseMessage response, string actionName)
        {
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                
                var content = await response.Content.ReadAsStringAsync();
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                TempData["SUErrorMessage"] = problemDetails?.Detail
                    ?? $"Ошибка уникальности при {actionName} предмета";
            }
            else
            {

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["POErrorMessage"] = $"Ошибка при {actionName} предмета: {response.StatusCode}";
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Subject/{id}", ct);
                if (response.IsSuccessStatusCode)
                    TempData["SUSuccessMessage"] = "Предмет успешно удалён!";
                else
                    TempData["SUErrorMessage"] = "Ошибка при удалении предмета.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}