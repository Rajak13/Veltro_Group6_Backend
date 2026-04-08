using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Line item within a purchase invoice.</summary>
public class PurchaseInvoiceItem
{
    [Key]
    public Guid ItemId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(PurchaseInvoice))]
    public Guid InvoiceId { get; set; }

    [ForeignKey(nameof(Part))]
    public Guid PartId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // Navigation
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
    public Part Part { get; set; } = null!;
}
