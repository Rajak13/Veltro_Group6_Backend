using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Part;

/// <summary>Payload for updating an existing part.</summary>
public class UpdatePartDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }

    [Range(0, int.MaxValue)]
    public int? StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int? LowStockThreshold { get; set; }

    public Guid? VendorId { get; set; }
}
