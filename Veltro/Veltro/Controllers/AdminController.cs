using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.Data;
using Veltro.DTOs.Request.Staff;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Controllers;

/// <summary>Admin-only endpoints for staff management.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly AppDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IUserRepository userRepo, AppDbContext context, ILogger<AdminController> logger)
    {
        _userRepo = userRepo;
        _context = context;
        _logger = logger;
    }

    /// <summary>Registers a new staff member.</summary>
    [HttpPost("staff")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
    {
        try
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest(ApiResponse<object>.Fail("Email already registered."));

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = PasswordHelper.Hash(dto.Password),
                Role = UserRole.Staff
            };
            await _context.Users.AddAsync(user);

            var staffRecord = new Staff { UserId = user.Id, Position = dto.Position };
            await _context.Staff.AddAsync(staffRecord);
            await _context.SaveChangesAsync();

            return StatusCode(201, ApiResponse<object>.Ok(new { user.Id, user.FullName, user.Email }, "Staff created."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create staff");
            return StatusCode(500, ApiResponse<object>.Fail("An error occurred."));
        }
    }

    /// <summary>Returns a list of all staff members.</summary>
    [HttpGet("staff")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStaff()
    {
        var staff = await _userRepo.GetByRoleAsync(UserRole.Staff);
        var result = staff.Select(u => new { u.Id, u.FullName, u.Email, u.IsActive, u.CreatedAt });
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Updates a staff member's details.</summary>
    [HttpPut("staff/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] CreateStaffDto dto)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null || user.Role != UserRole.Staff)
                return NotFound(ApiResponse<object>.Fail("Staff member not found."));

            user.FullName = dto.FullName;
            user.Email = dto.Email.ToLower();
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = PasswordHelper.Hash(dto.Password);

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(new { user.Id, user.FullName, user.Email }, "Staff updated."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update staff {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("An error occurred."));
        }
    }

    /// <summary>Deactivates a staff member (soft delete).</summary>
    [HttpDelete("staff/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateStaff(Guid id)
    {
        try
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null || user.Role != UserRole.Staff)
                return NotFound(ApiResponse<object>.Fail("Staff member not found."));

            user.IsActive = false;
            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(new { }, "Staff deactivated."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate staff {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("An error occurred."));
        }
    }
}
