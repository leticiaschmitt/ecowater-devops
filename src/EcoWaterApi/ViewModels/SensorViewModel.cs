using System.ComponentModel.DataAnnotations;

namespace EcoWaterApi.ViewModels;

public class SensorViewModel
{
    [Required(ErrorMessage = "O nome do sensor é obrigatório.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "A localização do sensor é obrigatória.")]
    [StringLength(150)]
    public string Location { get; set; } = string.Empty;

    [Range(1, 100000, ErrorMessage = "O limite máximo de consumo deve ser maior que zero.")]
    public decimal MaxConsumptionLimit { get; set; }

    public bool IsActive { get; set; } = true;
}