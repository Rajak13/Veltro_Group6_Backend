using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.DTOs.Request.Customer;
using Veltro.Helpers;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>Staff endpoints for customer registration, lookup, and search.</summary>
[ApiController]
[Route("api/staff")]
[Authorize(Roles = "Admin,Staff")]
public class StaffController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public StaffController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Returns a paginated list of all customers.
    /// Searches across both name and phone simultaneously.
    /// </summary>
    [HttpGet("customers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = await _customerService.GetCustomersAsync(search, page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Registers a new customer with an optional vehicle.</summary>
    [HttpPost("customers")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        try
        {
            var result = await _customerService.CreateCustomerAsync(dto);
            return StatusCode(201, ApiResponse<object>.Ok(result, "Customer registered."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>Returns a customer's profile and vehicle information.</summary>
    [HttpGet("customers/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        return customer == null
            ? NotFound(ApiResponse<object>.Fail("Customer not found."))
            : Ok(ApiResponse<object>.Ok(customer));
    }

    /// <summary>Returns a customer's full purchase and service history.</summary>
    [HttpGet("customers/{id:guid}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerHistory(Guid id)
    {
        var history = await _customerService.GetCustomerHistoryAsync(id);
        return history == null
            ? NotFound(ApiResponse<object>.Fail("Customer not found."))
            : Ok(ApiResponse<object>.Ok(history));
    }

    /// <summary>Searches customers by name, phone, ID, or vehicle registration number.</summary>
    [HttpGet("customers/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCustomers(
        [FromQuery] string q,
        [FromQuery] string type = "name",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _customerService.SearchCustomersAsync(q, type, page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
