using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.DTOs.Request.Vendor;
using Veltro.Helpers;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>Vendor management — Admin only.</summary>
[ApiController]
[Route("api/vendors")]
[Authorize(Roles = "Admin")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    /// <summary>Returns a paginated list of all vendors.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _vendorService.GetAllVendorsAsync(page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns a single vendor by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(id);
        return vendor == null
            ? NotFound(ApiResponse<object>.Fail("Vendor not found."))
            : Ok(ApiResponse<object>.Ok(vendor));
    }

    /// <summary>Creates a new vendor.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendorDto dto)
    {
        var vendor = await _vendorService.CreateVendorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = vendor.VendorId },
            ApiResponse<object>.Ok(vendor, "Vendor created."));
    }

    /// <summary>Updates an existing vendor.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVendorDto dto)
    {
        var vendor = await _vendorService.UpdateVendorAsync(id, dto);
        return vendor == null
            ? NotFound(ApiResponse<object>.Fail("Vendor not found."))
            : Ok(ApiResponse<object>.Ok(vendor, "Vendor updated."));
    }

    /// <summary>Soft-deletes a vendor (sets IsActive = false).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _vendorService.DeleteVendorAsync(id);
        return deleted
            ? Ok(ApiResponse<object>.Ok(new { }, "Vendor deactivated."))
            : NotFound(ApiResponse<object>.Fail("Vendor not found."));
    }
}
