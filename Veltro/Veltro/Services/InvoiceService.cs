using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.DTOs.Request.Invoice;
using Veltro.DTOs.Response.Invoice;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Repositories.Interfaces;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>
/// Handles purchase and sales invoice creation.
/// Applies loyalty discount, reduces stock, triggers low-stock notifications,
/// and sends invoice emails via MailKit.
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IPartRepository _partRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IUserRepository _userRepo;
    private readonly ILoyaltyService _loyaltyService;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository invoiceRepo, IPartRepository partRepo,
        ICustomerRepository customerRepo, IUserRepository userRepo,
        ILoyaltyService loyaltyService, INotificationService notificationService,
        IEmailService emailService, AppDbContext context,
        ILogger<InvoiceService> logger)
    {
        _invoiceRepo = invoiceRepo;
        _partRepo = partRepo;
        _customerRepo = customerRepo;
        _userRepo = userRepo;
        _loyaltyService = loyaltyService;
        _notificationService = notificationService;
        _emailService = emailService;
        _context = context;
        _logger = logger;
    }

    // ─── Purchase Invoices ───────────────────────────────────────────────────

    /// <summary>Creates a purchase invoice and increments stock for each item.</summary>
    public async Task<PurchaseInvoiceResponseDto> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDto dto)
    {
        try
        {
            var invoice = new PurchaseInvoice
            {
                VendorId = dto.VendorId,
                Notes = dto.Notes,
                PurchaseDate = DateTime.UtcNow,
                Items = new List<PurchaseInvoiceItem>()
            };

            decimal total = 0;
            foreach (var itemDto in dto.Items)
            {
                var part = await _partRepo.GetByIdAsync(itemDto.PartId)
                    ?? throw new KeyNotFoundException($"Part {itemDto.PartId} not found.");

                part.StockQuantity += itemDto.Quantity;
                part.UpdatedAt = DateTime.UtcNow;
                _partRepo.Update(part);

                var item = new PurchaseInvoiceItem
                {
                    PartId = itemDto.PartId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice
                };
                invoice.Items.Add(item);
                total += itemDto.Quantity * itemDto.UnitPrice;
            }

            invoice.TotalAmount = total;
            await _invoiceRepo.AddPurchaseInvoiceAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();

            return await MapPurchaseInvoiceAsync(invoice.InvoiceId);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Failed to create purchase invoice");
            throw;
        }
    }

    /// <summary>Returns a paginated list of all purchase invoices.</summary>
    public async Task<PagedResult<PurchaseInvoiceResponseDto>> GetAllPurchaseInvoicesAsync(int page, int pageSize)
    {
        try
        {
            var query = _invoiceRepo.GetPurchaseQueryable()
                .Select(i => new PurchaseInvoiceResponseDto
                {
                    InvoiceId = i.InvoiceId,
                    VendorId = i.VendorId,
                    VendorName = i.Vendor.Name,
                    TotalAmount = i.TotalAmount,
                    PurchaseDate = i.PurchaseDate,
                    Notes = i.Notes
                });
            return await PaginationHelper.PaginateAsync(query, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve purchase invoices");
            throw;
        }
    }

    /// <summary>Returns a single purchase invoice with all line items.</summary>
    public async Task<PurchaseInvoiceResponseDto?> GetPurchaseInvoiceByIdAsync(Guid id)
    {
        try
        {
            var invoice = await _invoiceRepo.GetPurchaseInvoiceByIdAsync(id);
            if (invoice == null) return null;
            return MapPurchaseDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve purchase invoice {Id}", id);
            throw;
        }
    }

    // ─── Sales Invoices ──────────────────────────────────────────────────────

    /// <summary>
    /// Creates a sales invoice. Reduces stock per item, applies 10% loyalty discount
    /// if subtotal > 5000, and triggers low-stock notifications where needed.
    /// </summary>
    public async Task<SalesInvoiceResponseDto> CreateSalesInvoiceAsync(CreateSalesInvoiceDto dto, Guid staffUserId)
    {
        try
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == staffUserId)
                ?? throw new KeyNotFoundException("Staff record not found for current user.");

            var invoice = new SalesInvoice
            {
                CustomerId = dto.CustomerId,
                StaffId = staff.StaffId,
                SaleDate = DateTime.UtcNow,
                Items = new List<SalesInvoiceItem>()
            };

            decimal subtotal = 0;
            var partsToCheck = new List<Part>();

            foreach (var itemDto in dto.Items)
            {
                var part = await _partRepo.GetByIdAsync(itemDto.PartId)
                    ?? throw new KeyNotFoundException($"Part {itemDto.PartId} not found.");

                if (part.StockQuantity < itemDto.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for '{part.Name}'. Available: {part.StockQuantity}.");

                part.StockQuantity -= itemDto.Quantity;
                part.UpdatedAt = DateTime.UtcNow;
                _partRepo.Update(part);
                partsToCheck.Add(part);

                invoice.Items.Add(new SalesInvoiceItem
                {
                    PartId = itemDto.PartId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = part.Price
                });
                subtotal += itemDto.Quantity * part.Price;
            }

            // Loyalty Program: 10% discount if subtotal > 5000
            var discount = _loyaltyService.CalculateDiscount(subtotal);
            invoice.DiscountApplied = discount;
            invoice.TotalAmount = subtotal - discount;

            await _invoiceRepo.AddSalesInvoiceAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();

            // Trigger low-stock notifications after stock has been reduced
            var admins = await _userRepo.GetByRoleAsync(UserRole.Admin);
            foreach (var part in partsToCheck.Where(p => p.StockQuantity < p.LowStockThreshold))
            {
                foreach (var admin in admins)
                {
                    await _notificationService.CreateNotificationAsync(
                        admin.Id,
                        $"Low stock alert: '{part.Name}' dropped to {part.StockQuantity} units.",
                        NotificationType.LowStock
                    );
                }
            }

            return (await GetSalesInvoiceByIdAsync(invoice.InvoiceId))!;
        }
        catch (Exception ex) when (ex is not KeyNotFoundException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to create sales invoice");
            throw;
        }
    }

    /// <summary>Returns a paginated list of all sales invoices.</summary>
    public async Task<PagedResult<SalesInvoiceResponseDto>> GetAllSalesInvoicesAsync(int page, int pageSize)
    {
        try
        {
            var query = _invoiceRepo.GetSalesQueryable()
                .Select(i => new SalesInvoiceResponseDto
                {
                    InvoiceId = i.InvoiceId,
                    CustomerId = i.CustomerId,
                    CustomerName = i.Customer.User.FullName,
                    CustomerEmail = i.Customer.User.Email,
                    StaffId = i.StaffId,
                    StaffName = i.Staff.User.FullName,
                    TotalAmount = i.TotalAmount,
                    DiscountApplied = i.DiscountApplied,
                    IsPaid = i.IsPaid,
                    PaidAt = i.PaidAt,
                    SaleDate = i.SaleDate
                });
            return await PaginationHelper.PaginateAsync(query, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve sales invoices");
            throw;
        }
    }

    /// <summary>Returns a single sales invoice with all line items.</summary>
    public async Task<SalesInvoiceResponseDto?> GetSalesInvoiceByIdAsync(Guid id)
    {
        try
        {
            var invoice = await _invoiceRepo.GetSalesInvoiceByIdAsync(id);
            if (invoice == null) return null;
            return MapSalesDto(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve sales invoice {Id}", id);
            throw;
        }
    }

    /// <summary>Sends a formatted HTML invoice email to the customer via MailKit.</summary>
    public async Task SendInvoiceEmailAsync(Guid invoiceId)
    {
        try
        {
            var invoice = await _invoiceRepo.GetSalesInvoiceByIdAsync(invoiceId)
                ?? throw new KeyNotFoundException("Invoice not found.");

            var itemRows = string.Join("", invoice.Items.Select(i =>
                $"<tr><td>{i.Part.Name}</td><td>{i.Quantity}</td><td>{i.UnitPrice:C}</td><td>{i.Quantity * i.UnitPrice:C}</td></tr>"));

            var html = $"""
                <h2>Veltro — Sales Invoice #{invoice.InvoiceId}</h2>
                <p>Dear {invoice.Customer.User.FullName},</p>
                <p>Thank you for your purchase on {invoice.SaleDate:dd MMM yyyy}.</p>
                <table border="1" cellpadding="6">
                  <thead><tr><th>Part</th><th>Qty</th><th>Unit Price</th><th>Total</th></tr></thead>
                  <tbody>{itemRows}</tbody>
                </table>
                <p>Discount Applied: {invoice.DiscountApplied:C}</p>
                <p><strong>Total Amount: {invoice.TotalAmount:C}</strong></p>
                <p>Status: {(invoice.IsPaid ? "Paid" : "Unpaid")}</p>
                <br/><p>Veltro Vehicle Parts</p>
                """;

            await _emailService.SendEmailAsync(
                invoice.Customer.User.Email,
                invoice.Customer.User.FullName,
                $"Your Veltro Invoice #{invoice.InvoiceId}",
                html
            );
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Failed to send invoice email for {Id}", invoiceId);
            throw;
        }
    }

    // ─── Mapping helpers ─────────────────────────────────────────────────────

    private async Task<PurchaseInvoiceResponseDto> MapPurchaseInvoiceAsync(Guid id)
    {
        var invoice = await _invoiceRepo.GetPurchaseInvoiceByIdAsync(id);
        return MapPurchaseDto(invoice!);
    }

    private static PurchaseInvoiceResponseDto MapPurchaseDto(PurchaseInvoice i) => new()
    {
        InvoiceId = i.InvoiceId,
        VendorId = i.VendorId,
        VendorName = i.Vendor?.Name ?? string.Empty,
        TotalAmount = i.TotalAmount,
        PurchaseDate = i.PurchaseDate,
        Notes = i.Notes,
        Items = i.Items.Select(item => new InvoiceItemDto
        {
            ItemId = item.ItemId,
            PartId = item.PartId,
            PartName = item.Part?.Name ?? string.Empty,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList()
    };

    private static SalesInvoiceResponseDto MapSalesDto(SalesInvoice i) => new()
    {
        InvoiceId = i.InvoiceId,
        CustomerId = i.CustomerId,
        CustomerName = i.Customer?.User?.FullName ?? string.Empty,
        CustomerEmail = i.Customer?.User?.Email ?? string.Empty,
        StaffId = i.StaffId,
        StaffName = i.Staff?.User?.FullName ?? string.Empty,
        TotalAmount = i.TotalAmount,
        DiscountApplied = i.DiscountApplied,
        IsPaid = i.IsPaid,
        PaidAt = i.PaidAt,
        SaleDate = i.SaleDate,
        Items = i.Items.Select(item => new InvoiceItemDto
        {
            ItemId = item.ItemId,
            PartId = item.PartId,
            PartName = item.Part?.Name ?? string.Empty,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList()
    };
}
