namespace EcoWaterApi.Models;

public class EnvironmentalReport
{
    public int Id { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal TotalConsumption { get; set; }
    public decimal EstimatedWaste { get; set; }
    public string SuggestedAction { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}