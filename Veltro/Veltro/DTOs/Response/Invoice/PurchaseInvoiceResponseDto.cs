namespace Veltro.DTOs.Response.Invoice;

/// <summary>Purchase invoice details returned in responses.</summary>
public class PurchaseInvoiceResponseDto
{
    public Guid InvoiceId { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

/// <summary>Line item within an invoice response.</summary>
public class InvoiceItemDto
{
    public Guid ItemId { get; set; }
    public Guid PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}
