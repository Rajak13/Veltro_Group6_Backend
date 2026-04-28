using Veltro.DTOs.Request.Invoice;
using Veltro.DTOs.Response.Invoice;
using Veltro.Helpers;

namespace Veltro.Services.Interfaces;

/// <summary>Invoice management service contract.</summary>
public interface IInvoiceService
{
    Task<PurchaseInvoiceResponseDto> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto dto);
    Task<PagedResult<PurchaseInvoiceResponseDto>> GetAllPurchaseInvoicesAsync(int page, int pageSize);
    Task<PurchaseInvoiceResponseDto?> GetPurchaseInvoiceByIdAsync(Guid id);

    Task<SalesInvoiceResponseDto> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto, Guid staffUserId);
    Task<PagedResult<SalesInvoiceResponseDto>> GetAllSalesInvoicesAsync(int page, int pageSize);
    Task<SalesInvoiceResponseDto?> GetSalesInvoiceByIdAsync(Guid id);
    Task SendInvoiceEmailAsync(Guid invoiceId);
}
