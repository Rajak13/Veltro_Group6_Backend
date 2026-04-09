using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.Helpers;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>Financial and customer reports — Admin and Staff access.</summary>
[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>Returns financial summary (sales, purchases, net profit, top parts) for daily/monthly/yearly period.</summary>
    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFinancialReport([FromQuery] string period = "monthly")
    {
        try
        {
            var report = await _reportService.GetFinancialReportAsync(period);
            return Ok(ApiResponse<object>.Ok(report));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Returns the top-spending customers.</summary>
    [HttpGet("customers/top-spenders")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetTopSpenders([FromQuery] int top = 10)
    {
        var result = await _reportService.GetTopSpendersAsync(top);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns customers who have made 3 or more purchases.</summary>
    [HttpGet("customers/regulars")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetRegularCustomers([FromQuery] int minPurchases = 3)
    {
        var result = await _reportService.GetRegularCustomersAsync(minPurchases);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns customers with overdue credit balances (unpaid > 1 month).</summary>
    [HttpGet("customers/overdue-credits")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetOverdueCredits()
    {
        var result = await _reportService.GetOverdueCreditsAsync();
        return Ok(ApiResponse<object>.Ok(result));
    }
}
