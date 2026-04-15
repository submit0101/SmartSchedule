using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Core.Service.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSchedule.Api.Controllers;

/// <summary>
/// Контроллер для формирования и выгрузки отчетов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExcelExportService _exportService;

    /// <summary>
    /// Конструктор с внедрением сервиса экспорта.
    /// </summary>
    public ExportController(IExcelExportService exportService)
    {
        _exportService = exportService;
    }

    /// <summary>
    /// Выгружает расписание указанной группы в формате Excel.
    /// </summary>
    /// <param name="groupId">ID группы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    [Authorize(Roles = "Admin, Dispatcher")]
    [HttpGet("excel/group/{groupId}")]
    public async Task<IActionResult> ExportGroupSchedule(int groupId, CancellationToken ct)
    {
        
        var (fileContent, groupName) = await _exportService.GenerateGroupScheduleAsync(groupId, ct);

        if (fileContent == null || fileContent.Length == 0)
        {
            return NotFound("Расписание для данной группы не найдено или пусто.");
        }

        
        return File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Schedule_{groupName}_{System.DateTime.Now:yyyyMMdd}.xlsx"
        );
    }
}
