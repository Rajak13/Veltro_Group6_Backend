using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.DTOs.Response.Report;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Generates financial and customer reports using projected queries for efficiency.</summary>
public class ReportService : IReportService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(AppDbContext context, ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Returns total sales, purchases, net profit, and top-selling parts for the given period.</summary>
    public async Task<FinancialReportDto> GetFinancialReportAsync(string period)
    {
        try
        {
            var now = DateTime.UtcNow;
            DateTime from = period.ToLower() switch
            {
                "daily"   => now.Date,
                "monthly" => new DateTime(now.Year, now.Month, 1),
                "yearly"  => new DateTime(now.Year, 1, 1),
                _         => throw new ArgumentException("Invalid period. Use daily, monthly, or yearly.")
            };

            // Use Select() to project only needed fields — never load full collections
            var totalSales = await _context.SalesInvoices
                .AsNoTracking()
                .Where(si => si.SaleDate >= from)
                .SumAsync(si => (decimal?)si.TotalAmount) ?? 0;

            var totalPurchases = await _context.PurchaseInvoices
                .AsNoTracking()
                .Where(pi => pi.PurchaseDate >= from)
                .SumAsync(pi => (decimal?)pi.TotalAmount) ?? 0;

            var topParts = await _context.SalesInvoiceItems
                .AsNoTracking()
                .Where(item => item.SalesInvoice.SaleDate >= from)
                .GroupBy(item => new { item.PartId, item.Part.Name })
                .Select(g => new TopSellingPartDto
                {
                    PartId = g.Key.PartId,
                    PartName = g.Key.Name,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(10)
                .ToListAsync();

            return new FinancialReportDto
            {
                Period = period,
                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                TopSellingParts = topParts
            };
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogError(ex, "Failed to generate financial report for period '{Period}'", period);
            throw;
        }
    }

    /// <summary>Returns the top N customers by total amount spent.</summary>
    public async Task<IEnumerable<CustomerReportDto>> GetTopSpendersAsync(int top = 10)
    {
        try
        {
            return await _context.Customers
                .AsNoTracking()
                .Select(c => new CustomerReportDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.User.FullName,
                    Email = c.User.Email,
                    TotalSpent = c.SalesInvoices.Sum(si => si.TotalAmount),
                    InvoiceCount = c.SalesInvoices.Count(),
                    CreditBalance = c.CreditBalance,
                    LastPurchaseDate = c.SalesInvoices
                        .OrderByDescending(si => si.SaleDate)
                        .Select(si => (DateTime?)si.SaleDate)
                        .FirstOrDefault()
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(top)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get top spenders");
            throw;
        }
    }

    /// <summary>Returns customers with at least minPurchases completed invoices.</summary>
    public async Task<IEnumerable<CustomerReportDto>> GetRegularCustomersAsync(int minPurchases = 3)
    {
        try
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.SalesInvoices.Count() >= minPurchases)
                .Select(c => new CustomerReportDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.User.FullName,
                    Email = c.User.Email,
                    TotalSpent = c.SalesInvoices.Sum(si => si.TotalAmount),
                    InvoiceCount = c.SalesInvoices.Count(),
                    CreditBalance = c.CreditBalance,
                    LastPurchaseDate = c.SalesInvoices
                        .OrderByDescending(si => si.SaleDate)
                        .Select(si => (DateTime?)si.SaleDate)
                        .FirstOrDefault()
                })
                .OrderByDescending(c => c.InvoiceCount)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get regular customers");
            throw;
        }
    }

    /// <summary>Returns customers with unpaid invoices older than 1 month (overdue credit).</summary>
    public async Task<IEnumerable<CustomerReportDto>> GetOverdueCreditsAsync()
    {
        try
        {
            var cutoff = DateTime.UtcNow.AddMonths(-1);
            return await _context.Customers
                .AsNoTracking()
                .Where(c => c.CreditBalance > 0 &&
                            c.SalesInvoices.Any(si => !si.IsPaid && si.SaleDate < cutoff))
                .Select(c => new CustomerReportDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.User.FullName,
                    Email = c.User.Email,
                    TotalSpent = c.SalesInvoices.Sum(si => si.TotalAmount),
                    InvoiceCount = c.SalesInvoices.Count(),
                    CreditBalance = c.CreditBalance,
                    LastPurchaseDate = c.SalesInvoices
                        .OrderByDescending(si => si.SaleDate)
                        .Select(si => (DateTime?)si.SaleDate)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get overdue credits");
            throw;
        }
    }
}
