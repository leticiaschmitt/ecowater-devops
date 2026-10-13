using EcoWaterApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcoWaterApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("environmental-impact")]
    public async Task<IActionResult> GetEnvironmentalImpact([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var report = await _reportService.GetEnvironmentalImpactAsync(page, pageSize);
        return Ok(report);
    }

    [HttpGet("water-saving")]
    public async Task<IActionResult> GetWaterSaving()
    {
        var report = await _reportService.GetWaterSavingAsync();
        return Ok(report);
    }
}