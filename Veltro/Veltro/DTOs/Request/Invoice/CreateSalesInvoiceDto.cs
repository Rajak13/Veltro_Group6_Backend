using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Invoice;

/// <summary>Payload for creating a sales invoice (Staff selling to Customer).</summary>
public class CreateSalesInvoiceDto
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required, MinLength(1)]
    public List<SalesInvoiceItemDto> Items { get; set; } = new();
}

/// <summary>Single line item within a sales invoice.</summary>
public class SalesInvoiceItemDto
{
    [Required]
    public Guid PartId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
