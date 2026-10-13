using EcoWaterApi.Data;
using EcoWaterApi.Helpers;
using EcoWaterApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoWaterApi.Services;

public class AlertService
{
    private readonly AppDbContext _context;

    public AlertService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<Alert>> GetAllAsync(int page, int pageSize)
    {
        var query = _context.Alerts
            .Include(a => a.Sensor)
            .AsNoTracking();

        var totalItems = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<Alert>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = items
        };
    }

    public async Task<bool> ResolveAsync(int id)
    {
        var alert = await _context.Alerts.FindAsync(id);

        if (alert == null)
            return false;

        alert.IsResolved = true;
        alert.ResolvedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}