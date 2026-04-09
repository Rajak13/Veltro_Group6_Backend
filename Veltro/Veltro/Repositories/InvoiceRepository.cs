using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>Invoice repository handling both purchase and sales invoices.</summary>
public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(Guid id) =>
        await _context.PurchaseInvoices
            .Include(i => i.Vendor)
            .Include(i => i.Items).ThenInclude(item => item.Part)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);

    public async Task<IEnumerable<PurchaseInvoice>> GetAllPurchaseInvoicesAsync() =>
        await _context.PurchaseInvoices
            .AsNoTracking()
            .Include(i => i.Vendor)
            .Include(i => i.Items).ThenInclude(item => item.Part)
            .ToListAsync();

    public async Task AddPurchaseInvoiceAsync(PurchaseInvoice invoice) =>
        await _context.PurchaseInvoices.AddAsync(invoice);

    public async Task<SalesInvoice?> GetSalesInvoiceByIdAsync(Guid id) =>
        await _context.SalesInvoices
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Include(i => i.Staff).ThenInclude(s => s.User)
            .Include(i => i.Items).ThenInclude(item => item.Part)
            .FirstOrDefaultAsync(i => i.InvoiceId == id);

    public async Task<IEnumerable<SalesInvoice>> GetAllSalesInvoicesAsync() =>
        await _context.SalesInvoices
            .AsNoTracking()
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Include(i => i.Staff).ThenInclude(s => s.User)
            .Include(i => i.Items).ThenInclude(item => item.Part)
            .ToListAsync();

    public async Task AddSalesInvoiceAsync(SalesInvoice invoice) =>
        await _context.SalesInvoices.AddAsync(invoice);

    public IQueryable<SalesInvoice> GetSalesQueryable() =>
        _context.SalesInvoices.AsNoTracking()
            .Include(i => i.Customer).ThenInclude(c => c.User)
            .Include(i => i.Items).ThenInclude(item => item.Part);

    public IQueryable<PurchaseInvoice> GetPurchaseQueryable() =>
        _context.PurchaseInvoices.AsNoTracking()
            .Include(i => i.Items).ThenInclude(item => item.Part);

    public async Task<bool> SaveChangesAsync() =>
        await _context.SaveChangesAsync() > 0;
}
