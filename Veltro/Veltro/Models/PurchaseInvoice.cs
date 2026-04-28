using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Invoice for Admin purchasing stock from a Vendor.</summary>
public class PurchaseInvoice
{
    [Key]
    public Guid InvoiceId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Vendor))]
    public Guid VendorId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }

    // Navigation
    public Vendor Vendor { get; set; } = null!;
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
}
