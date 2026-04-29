using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.DTOs.Request.Part;
using Veltro.Helpers;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>Parts inventory management — Admin only.</summary>
[ApiController]
[Route("api/parts")]
[Authorize(Roles = "Admin")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    /// <summary>Returns a paginated list of all parts with current stock levels.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _partService.GetAllPartsAsync(page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns a single part by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var part = await _partService.GetPartByIdAsync(id);
        return part == null
            ? NotFound(ApiResponse<object>.Fail("Part not found."))
            : Ok(ApiResponse<object>.Ok(part));
    }

    /// <summary>Creates a new part in inventory.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePartDto dto)
    {
        var part = await _partService.CreatePartAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = part.PartId },
            ApiResponse<object>.Ok(part, "Part created."));
    }

    /// <summary>Updates an existing part's details.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartDto dto)
    {
        var part = await _partService.UpdatePartAsync(id, dto);
        return part == null
            ? NotFound(ApiResponse<object>.Fail("Part not found."))
            : Ok(ApiResponse<object>.Ok(part, "Part updated."));
    }

    /// <summary>Deletes a part from inventory.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _partService.DeletePartAsync(id);
        return deleted
            ? Ok(ApiResponse<object>.Ok(new { }, "Part deleted."))
            : NotFound(ApiResponse<object>.Fail("Part not found."));
    }

    /// <summary>Searches parts by name or description.</summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _partService.SearchPartsAsync(q, page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Returns parts that are below their low stock threshold.</summary>
    [HttpGet("low-stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLowStock([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _partService.GetLowStockPartsAsync(page, pageSize);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
