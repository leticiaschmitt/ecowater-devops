using System.ComponentModel.DataAnnotations;

namespace EcoWaterApi.ViewModels;

public class AlertViewModel
{
    [Required]
    public int SensorId { get; set; }

    [Required]
    [StringLength(250)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public string Severity { get; set; } = "Medium";
}