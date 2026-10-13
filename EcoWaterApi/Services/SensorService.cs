using EcoWaterApi.Data;
using EcoWaterApi.Helpers;
using EcoWaterApi.Models;
using EcoWaterApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoWaterApi.Services;

public class SensorService
{
    private readonly AppDbContext _context;

    public SensorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<Sensor>> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Sensors.AsNoTracking();

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderBy(sensor => sensor.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<Sensor>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = items
        };
    }

    public async Task<Sensor?> GetByIdAsync(int id)
    {
        return await _context.Sensors.FindAsync(id);
    }

    public async Task<Sensor> CreateAsync(SensorViewModel viewModel)
    {
        var sensor = new Sensor
        {
            Name = viewModel.Name,
            Location = viewModel.Location,
            MaxConsumptionLimit = viewModel.MaxConsumptionLimit,
            IsActive = viewModel.IsActive
        };

        _context.Sensors.Add(sensor);
        await _context.SaveChangesAsync();

        return sensor;
    }

    public async Task<bool> UpdateAsync(int id, SensorViewModel viewModel)
    {
        var sensor = await _context.Sensors.FindAsync(id);

        if (sensor == null)
            return false;

        sensor.Name = viewModel.Name;
        sensor.Location = viewModel.Location;
        sensor.MaxConsumptionLimit = viewModel.MaxConsumptionLimit;
        sensor.IsActive = viewModel.IsActive;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sensor = await _context.Sensors.FindAsync(id);

        if (sensor == null)
            return false;

        _context.Sensors.Remove(sensor);
        await _context.SaveChangesAsync();

        return true;
    }
}