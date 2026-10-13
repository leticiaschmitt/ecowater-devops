using EcoWaterApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoWaterApi.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertController : ControllerBase
{
    private readonly AlertService _alertService;

    public AlertController(AlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var alerts = await _alertService.GetAllAsync(page, pageSize);
        return Ok(alerts);
    }

    [Authorize]
    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> Resolve(int id)
    {
        var resolved = await _alertService.ResolveAsync(id);

        if (!resolved)
            return NotFound(new { message = "Alerta não encontrado." });

        return Ok(new { message = "Alerta resolvido com sucesso." });
    }
}