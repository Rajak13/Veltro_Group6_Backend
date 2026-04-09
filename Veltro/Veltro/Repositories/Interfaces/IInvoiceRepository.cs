using Veltro.Models;

namespace Veltro.Repositories.Interfaces;

/// <summary>Invoice-specific repository operations.</summary>
public interface IInvoiceRepository
{
    Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(Guid id);
    Task<IEnumerable<PurchaseInvoice>> GetAllPurchaseInvoicesAsync();
    Task AddPurchaseInvoiceAsync(PurchaseInvoice invoice);

    Task<SalesInvoice?> GetSalesInvoiceByIdAsync(Guid id);
    Task<IEnumerable<SalesInvoice>> GetAllSalesInvoicesAsync();
    Task AddSalesInvoiceAsync(SalesInvoice invoice);

    IQueryable<SalesInvoice> GetSalesQueryable();
    IQueryable<PurchaseInvoice> GetPurchaseQueryable();

    Task<bool> SaveChangesAsync();
}
