using EcoWaterApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoWaterApi.Services;

public class ReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetEnvironmentalImpactAsync(int page, int pageSize)
    {
        var totalItems = await _context.ConsumptionReadings.CountAsync();

        var readings = await _context.ConsumptionReadings
            .Include(c => c.Sensor)
            .OrderByDescending(c => c.ReadingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                Sensor = c.Sensor != null ? c.Sensor.Name : "Sensor não informado",
                c.ConsumptionLiters,
                c.ReadingDate,
                c.HasLeakSuspicion
            })
            .ToListAsync();

        return new
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = readings
        };
    }

    public async Task<object> GetWaterSavingAsync()
    {
        var totalConsumption = await _context.ConsumptionReadings
            .SumAsync(c => c.ConsumptionLiters);

        var suspectedWaste = await _context.ConsumptionReadings
            .Where(c => c.HasLeakSuspicion)
            .SumAsync(c => c.ConsumptionLiters);

        return new
        {
            TotalConsumptionLiters = totalConsumption,
            SuspectedWasteLiters = suspectedWaste,
            SustainabilityMessage = "Acompanhar o consumo de água ajuda a reduzir desperdícios e preservar recursos naturais."
        };
    }
}