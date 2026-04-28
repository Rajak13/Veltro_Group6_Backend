using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.DTOs.Request.Invoice;
using Veltro.Helpers;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>Purchase and sales invoice management.</summary>
[ApiController]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    // ─── Purchase Invoices (Admin) ────────────────────────────────────────────

    /// <summary>Creates a purchase invoice (Admin buying stock from a Vendor). Updates StockQuantity.</summary>
    [HttpPost("purchase")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseInvoiceDto dto)
    {
        try
        {
            var result = await _invoiceService.CreatePurchaseInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetPurchaseById), new { id = result.InvoiceId },
                ApiResponse<object>.Ok(result, "Purchase invoice created."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Returns a paginated list of all purchase invoices.</summary>
    [HttpGet("purchase")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllPurchase([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _invoiceService.GetAllPurchaseInvoicesAsync(page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns a single purchase invoice by ID.</summary>
    [HttpGet("purchase/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPurchaseById(Guid id)
    {
        var invoice = await _invoiceService.GetPurchaseInvoiceByIdAsync(id);
        return invoice == null
            ? NotFound(ApiResponse<object>.Fail("Invoice not found."))
            : Ok(ApiResponse<object>.Ok(invoice));
    }

    // ─── Sales Invoices (Staff) ───────────────────────────────────────────────

    /// <summary>
    /// Creates a sales invoice. Reduces stock, applies 10% loyalty discount if subtotal > 5000,
    /// and triggers low-stock notifications.
    /// </summary>
    [HttpPost("sales")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> CreateSales([FromBody] CreateSalesInvoiceDto dto)
    {
        try
        {
            var staffUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _invoiceService.CreateSalesInvoiceAsync(dto, staffUserId);
            return CreatedAtAction(nameof(GetSalesById), new { id = result.InvoiceId },
                ApiResponse<object>.Ok(result, "Sales invoice created."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Returns a paginated list of all sales invoices.</summary>
    [HttpGet("sales")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAllSales([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _invoiceService.GetAllSalesInvoicesAsync(page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns a single sales invoice by ID.</summary>
    [HttpGet("sales/{id:guid}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetSalesById(Guid id)
    {
        var invoice = await _invoiceService.GetSalesInvoiceByIdAsync(id);
        return invoice == null
            ? NotFound(ApiResponse<object>.Fail("Invoice not found."))
            : Ok(ApiResponse<object>.Ok(invoice));
    }

    /// <summary>Sends the sales invoice as a formatted HTML email to the customer via MailKit.</summary>
    [HttpPost("sales/{id:guid}/send-email")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> SendEmail(Guid id)
    {
        try
        {
            await _invoiceService.SendInvoiceEmailAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Invoice email sent."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
