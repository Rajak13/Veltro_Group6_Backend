using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Invoice for Staff selling parts to a Customer. Includes loyalty discount logic.</summary>
public class SalesInvoice
{
    [Key]
    public Guid InvoiceId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(Staff))]
    public Guid StaffId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    /// <summary>10% discount applied automatically if subtotal > 5000 (Loyalty Program).</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountApplied { get; set; } = 0;

    public bool IsPaid { get; set; } = false;

    public DateTime? PaidAt { get; set; }

    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Staff Staff { get; set; } = null!;
    public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
}
