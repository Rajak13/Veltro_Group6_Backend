namespace Veltro.DTOs.Response.Customer;

/// <summary>Customer purchase and service history.</summary>
public class CustomerHistoryResponseDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<PurchaseHistoryItemDto> Purchases { get; set; } = new();
    public List<AppointmentHistoryItemDto> Appointments { get; set; } = new();
}

/// <summary>Summary of a single sales invoice in history.</summary>
public class PurchaseHistoryItemDto
{
    public Guid InvoiceId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public bool IsPaid { get; set; }
}

/// <summary>Summary of a single appointment in history.</summary>
public class AppointmentHistoryItemDto
{
    public Guid AppointmentId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
