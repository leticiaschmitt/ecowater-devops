namespace EcoWaterApi.Models;

public class ConsumptionReading
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public Sensor? Sensor { get; set; }
    public decimal ConsumptionLiters { get; set; }
    public DateTime ReadingDate { get; set; } = DateTime.UtcNow;
    public bool HasLeakSuspicion { get; set; }
}