namespace EcoWaterApi.Models;

public class Sensor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal MaxConsumptionLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ConsumptionReading> ConsumptionReadings { get; set; } = new List<ConsumptionReading>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}