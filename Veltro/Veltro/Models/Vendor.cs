using System.ComponentModel.DataAnnotations;

namespace Veltro.Models;

/// <summary>Vendor supplying parts to the system.</summary>
public class Vendor
{
    [Key]
    public Guid VendorId { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Part> Parts { get; set; } = new List<Part>();
    public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
}
