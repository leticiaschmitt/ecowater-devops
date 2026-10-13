using EcoWaterApi.Data;
using EcoWaterApi.Helpers;
using EcoWaterApi.Models;
using EcoWaterApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoWaterApi.Services;

public class ConsumptionService
{
    private readonly AppDbContext _context;

    public ConsumptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<ConsumptionReading>> GetAllAsync(int page, int pageSize)
    {
        var query = _context.ConsumptionReadings
            .Include(c => c.Sensor)
            .AsNoTracking();

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.ReadingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<ConsumptionReading>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = items
        };
    }

    public async Task<ConsumptionReading?> GetByIdAsync(int id)
    {
        return await _context.ConsumptionReadings
            .Include(c => c.Sensor)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ConsumptionReading> CreateAsync(ConsumptionViewModel viewModel)
    {
        var sensor = await _context.Sensors.FindAsync(viewModel.SensorId);

        if (sensor == null)
            throw new Exception("Sensor não encontrado.");

        var hasLeak = viewModel.ConsumptionLiters > sensor.MaxConsumptionLimit;

        var reading = new ConsumptionReading
        {
            SensorId = viewModel.SensorId,
            ConsumptionLiters = viewModel.ConsumptionLiters,
            ReadingDate = viewModel.ReadingDate,
            HasLeakSuspicion = hasLeak
        };

        _context.ConsumptionReadings.Add(reading);

        if (hasLeak)
        {
            _context.Alerts.Add(new Alert
            {
                SensorId = viewModel.SensorId,
                Message = "Consumo acima do limite permitido.",
                Severity = "High",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        return reading;
    }
}