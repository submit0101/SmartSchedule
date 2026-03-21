using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTO.CabinetDTO;

namespace WebSmartSchedule.Pages.Cabinet
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<ResponseCabinetDto> Cabinet { get; set; } = new();
        public List<ShortBuildingDto> Buildings { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? BuildingNumber { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool Descending { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task OnGetAsync(CancellationToken ct)
        {
        
            var searchUrl = "api/Cabinet/search";

            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(SearchTerm))
                queryParams.Add("searchTerm", SearchTerm);

            if (BuildingNumber.HasValue)
                queryParams.Add("buildingNumber", BuildingNumber.Value.ToString());

            if (!string.IsNullOrEmpty(SortBy))
                queryParams.Add("sortBy", SortBy);

            if (Descending)
                queryParams.Add("descending", "true");

            // Формируем URL с параметрами
            if (queryParams.Any())
            {
                searchUrl += "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            }

            // Получаем кабинеты через search endpoint
            var response = await _httpClient.GetAsync(searchUrl, ct);
            if (response.IsSuccessStatusCode)
            {
                Cabinet = await response.Content.ReadFromJsonAsync<List<ResponseCabinetDto>>(ct) ?? new();
            }
            else
            {
                Console.WriteLine($"Ошибка при поиске кабинетов: {response.StatusCode}");
            }

         
            var buildingResponse = await _httpClient.GetAsync("api/Building/Short", ct);
            if (buildingResponse.IsSuccessStatusCode)
            {
                Buildings = await buildingResponse.Content.ReadFromJsonAsync<List<ShortBuildingDto>>(ct) ?? new();
            }
        }
        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateCabinetDto dto, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Cabinet", dto, ct);

                if (response.IsSuccessStatusCode)
                {
                    TempData["CASuccessMessage"] = "Кабинет успешно добавлен!";
                    return RedirectToPage();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(ct);
                    TempData["CAErrorMessage"] = errorContent?["message"]?.ToString() ??
                        "Кабинет с таким номером уже существует в этом здании";
                }
                else
                {
                    TempData["CAErrorMessage"] = $"Ошибка при создании кабинета: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["CAErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }

     public async Task<IActionResult> OnPostUpdateAsync(
     [FromForm] int id,
     [FromForm] UpdateCabinetDto dto,
     CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Cabinet/{id}", dto, ct);

                if (response.IsSuccessStatusCode)
                {
                    TempData["CASuccessMessage"] = "Кабинет успешно обновлен!";
                    return RedirectToPage();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(ct);
                    TempData["CAErrorMessage"] = errorContent?["message"]?.ToString() ??
                        "Кабинет с таким номером уже существует в этом здании";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["CAErrorMessage"] = "Кабинет не найден";
                }
                else
                {
                    TempData["CAErrorMessage"] = $"Ошибка при обновлении: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["CAErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
     }

        public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Cabinet/{id}", ct);

                if (response.IsSuccessStatusCode)
                {
                    TempData["CASuccessMessage"] = "Кабинет успешно удален!";
                }
                else
                {
                    TempData["CAErrorMessage"] = "Ошибка при удалении кабинета";
                }
            }
            catch (Exception ex)
            {
                TempData["CAErrorMessage"] = $"Ошибка: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
