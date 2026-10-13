using EcoWaterApi.Services;
using EcoWaterApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoWaterApi.Controllers;

[ApiController]
[Route("api/sensors")]
public class SensorController : ControllerBase
{
    private readonly SensorService _sensorService;

    public SensorController(SensorService sensorService)
    {
        _sensorService = sensorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var sensors = await _sensorService.GetAllAsync(page, pageSize);
        return Ok(sensors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sensor = await _sensorService.GetByIdAsync(id);

        if (sensor == null)
            return NotFound(new { message = "Sensor não encontrado." });

        return Ok(sensor);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SensorViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var sensor = await _sensorService.CreateAsync(viewModel);

        return CreatedAtAction(nameof(GetById), new { id = sensor.Id }, sensor);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SensorViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _sensorService.UpdateAsync(id, viewModel);

        if (!updated)
            return NotFound(new { message = "Sensor não encontrado." });

        return Ok(new { message = "Sensor atualizado com sucesso." });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _sensorService.DeleteAsync(id);

        if (!deleted)
            return NotFound(new { message = "Sensor não encontrado." });

        return Ok(new { message = "Sensor removido com sucesso." });
    }
}