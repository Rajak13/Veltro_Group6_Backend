using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Line item within a sales invoice.</summary>
public class SalesInvoiceItem
{
    [Key]
    public Guid ItemId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(SalesInvoice))]
    public Guid InvoiceId { get; set; }

    [ForeignKey(nameof(Part))]
    public Guid PartId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    // Navigation
    public SalesInvoice SalesInvoice { get; set; } = null!;
    public Part Part { get; set; } = null!;
}
