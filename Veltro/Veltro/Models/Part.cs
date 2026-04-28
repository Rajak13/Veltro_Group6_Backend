using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Vehicle part inventory item.</summary>
public class Part
{
    [Key]
    public Guid PartId { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>Current stock level. Admin notified when below LowStockThreshold.</summary>
    public int StockQuantity { get; set; } = 0;

    /// <summary>Minimum stock level before triggering low stock notification.</summary>
    public int LowStockThreshold { get; set; } = 10;

    [ForeignKey(nameof(Vendor))]
    public Guid VendorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Vendor Vendor { get; set; } = null!;
    public ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();
    public ICollection<SalesInvoiceItem> SalesInvoiceItems { get; set; } = new List<SalesInvoiceItem>();
}
