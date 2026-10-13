using EcoWaterApi.Services;
using EcoWaterApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoWaterApi.Controllers;

[ApiController]
[Route("api/consumptions")]
public class ConsumptionController : ControllerBase
{
    private readonly ConsumptionService _consumptionService;

    public ConsumptionController(ConsumptionService consumptionService)
    {
        _consumptionService = consumptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var consumptions = await _consumptionService.GetAllAsync(page, pageSize);
        return Ok(consumptions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consumption = await _consumptionService.GetByIdAsync(id);

        if (consumption == null)
            return NotFound(new { message = "Leitura de consumo não encontrada." });

        return Ok(consumption);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConsumptionViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var consumption = await _consumptionService.CreateAsync(viewModel);
            return CreatedAtAction(nameof(GetById), new { id = consumption.Id }, consumption);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}