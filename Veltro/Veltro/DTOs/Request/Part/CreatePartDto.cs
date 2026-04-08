using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Part;

/// <summary>Payload for creating a new part.</summary>
public class CreatePartDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;

    [Range(1, int.MaxValue)]
    public int LowStockThreshold { get; set; } = 10;

    [Required]
    public Guid VendorId { get; set; }
}
