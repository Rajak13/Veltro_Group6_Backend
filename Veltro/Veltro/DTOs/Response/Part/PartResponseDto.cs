namespace Veltro.DTOs.Response.Part;

/// <summary>Part details returned in list and detail responses.</summary>
public class PartResponseDto
{
    public Guid PartId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public bool IsLowStock => StockQuantity < LowStockThreshold;
    public string VendorName { get; set; } = string.Empty;
    public Guid VendorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
