namespace EcoWaterApi.Models;

public class Alert
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public Sensor? Sensor { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium";
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}