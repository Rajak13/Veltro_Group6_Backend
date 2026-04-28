namespace Veltro.DTOs.Response.Report;

/// <summary>Customer spending summary for reports.</summary>
public class CustomerReportDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int InvoiceCount { get; set; }
    public decimal CreditBalance { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
}
