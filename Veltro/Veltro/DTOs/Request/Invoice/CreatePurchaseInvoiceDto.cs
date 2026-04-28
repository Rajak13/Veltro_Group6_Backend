using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Invoice;

/// <summary>Payload for creating a purchase invoice (Admin buying from Vendor).</summary>
public class CreatePurchaseInvoiceDto
{
    [Required]
    public Guid VendorId { get; set; }

    public string? Notes { get; set; }

    [Required, MinLength(1)]
    public List<PurchaseInvoiceItemDto> Items { get; set; } = new();
}

/// <summary>Single line item within a purchase invoice.</summary>
public class PurchaseInvoiceItemDto
{
    [Required]
    public Guid PartId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}
