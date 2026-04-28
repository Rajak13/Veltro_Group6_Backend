namespace Veltro.DTOs.Response.Invoice;

/// <summary>Sales invoice details returned in responses.</summary>
public class SalesInvoiceResponseDto
{
    public Guid InvoiceId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Guid StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime SaleDate { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}
