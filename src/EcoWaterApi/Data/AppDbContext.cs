using EcoWaterApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoWaterApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<ConsumptionReading> ConsumptionReadings => Set<ConsumptionReading>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<EnvironmentalReport> EnvironmentalReports => Set<EnvironmentalReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sensor>()
            .HasMany(sensor => sensor.ConsumptionReadings)
            .WithOne(reading => reading.Sensor)
            .HasForeignKey(reading => reading.SensorId);

        modelBuilder.Entity<Sensor>()
            .HasMany(sensor => sensor.Alerts)
            .WithOne(alert => alert.Sensor)
            .HasForeignKey(alert => alert.SensorId);
    }
}