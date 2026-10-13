using System.ComponentModel.DataAnnotations;

namespace EcoWaterApi.ViewModels;

public class ConsumptionViewModel
{
    [Required]
    public int SensorId { get; set; }

    [Range(1, 1000000, ErrorMessage = "O consumo em litros deve ser maior que zero.")]
    public decimal ConsumptionLiters { get; set; }

    public DateTime ReadingDate { get; set; } = DateTime.UtcNow;
}