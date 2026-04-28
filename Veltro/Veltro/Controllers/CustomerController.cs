using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veltro.Data;
using Veltro.DTOs.Request.Appointment;
using Veltro.DTOs.Request.Customer;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Services.Interfaces;

namespace Veltro.Controllers;

/// <summary>Customer self-service endpoints — profile, vehicles, appointments, part requests, and reviews.</summary>
[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IAppointmentService _appointmentService;
    private readonly AppDbContext _context;

    public CustomerController(ICustomerService customerService,
        IAppointmentService appointmentService, AppDbContext context)
    {
        _customerService = customerService;
        _appointmentService = appointmentService;
        _context = context;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ─── Profile ──────────────────────────────────────────────────────────────

    /// <summary>Returns the authenticated customer's profile.</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        return customer == null
            ? NotFound(ApiResponse<object>.Fail("Profile not found."))
            : Ok(ApiResponse<object>.Ok(customer));
    }

    /// <summary>Updates the authenticated customer's profile.</summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerDto dto)
    {
        var customer = await _customerService.UpdateCustomerAsync(CurrentUserId, dto);
        return customer == null
            ? NotFound(ApiResponse<object>.Fail("Profile not found."))
            : Ok(ApiResponse<object>.Ok(customer, "Profile updated."));
    }

    // ─── Vehicles ─────────────────────────────────────────────────────────────

    /// <summary>Adds a vehicle to the authenticated customer's profile.</summary>
    [HttpPost("vehicles")]
    public async Task<IActionResult> AddVehicle([FromBody] CreateVehicleDto dto)
    {
        await _customerService.AddVehicleAsync(CurrentUserId, dto);
        return StatusCode(201, ApiResponse<object>.Ok(new { }, "Vehicle added."));
    }

    /// <summary>Updates an existing vehicle on the customer's profile.</summary>
    [HttpPut("vehicles/{id:guid}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] CreateVehicleDto dto)
    {
        try
        {
            await _customerService.UpdateVehicleAsync(id, dto);
            return Ok(ApiResponse<object>.Ok(new { }, "Vehicle updated."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // ─── Appointments ─────────────────────────────────────────────────────────

    /// <summary>Books a new service appointment.</summary>
    [HttpPost("/api/appointments")]
    public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto dto)
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var appointment = await _appointmentService.CreateAppointmentAsync(customer.CustomerId, dto);
        return StatusCode(201, ApiResponse<object>.Ok(new { appointment.AppointmentId }, "Appointment booked."));
    }

    /// <summary>Returns all appointments for the authenticated customer.</summary>
    [HttpGet("/api/appointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var appointments = await _appointmentService.GetCustomerAppointmentsAsync(customer.CustomerId);
        return Ok(ApiResponse<object>.Ok(appointments));
    }

    /// <summary>Cancels an appointment by ID.</summary>
    [HttpPut("/api/appointments/{id:guid}")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var cancelled = await _appointmentService.CancelAppointmentAsync(id, customer.CustomerId);
        return cancelled
            ? Ok(ApiResponse<object>.Ok(new { }, "Appointment cancelled."))
            : NotFound(ApiResponse<object>.Fail("Appointment not found or not owned by you."));
    }

    // ─── Part Requests ────────────────────────────────────────────────────────

    /// <summary>Submits a request for an unavailable part.</summary>
    [HttpPost("/api/part-requests")]
    public async Task<IActionResult> RequestPart([FromBody] PartRequestDto dto)
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var request = new PartRequest
        {
            CustomerId = customer.CustomerId,
            PartName = dto.PartName,
            Description = dto.Description
        };
        await _context.PartRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        return StatusCode(201, ApiResponse<object>.Ok(new { request.RequestId }, "Part request submitted."));
    }

    // ─── Reviews ──────────────────────────────────────────────────────────────

    /// <summary>Submits a service review with a 1–5 star rating.</summary>
    [HttpPost("/api/reviews")]
    public async Task<IActionResult> SubmitReview([FromBody] ReviewDto dto)
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var review = new Review
        {
            CustomerId = customer.CustomerId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return StatusCode(201, ApiResponse<object>.Ok(new { review.ReviewId }, "Review submitted."));
    }

    // ─── History ──────────────────────────────────────────────────────────────

    /// <summary>Returns the authenticated customer's purchase history.</summary>
    [HttpGet("history/purchases")]
    public async Task<IActionResult> GetPurchaseHistory()
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var history = await _customerService.GetCustomerHistoryAsync(customer.CustomerId);
        return Ok(ApiResponse<object>.Ok(history?.Purchases));
    }

    /// <summary>Returns the authenticated customer's appointment history.</summary>
    [HttpGet("history/appointments")]
    public async Task<IActionResult> GetAppointmentHistory()
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(CurrentUserId);
        if (customer == null) return NotFound(ApiResponse<object>.Fail("Customer not found."));

        var history = await _customerService.GetCustomerHistoryAsync(customer.CustomerId);
        return Ok(ApiResponse<object>.Ok(history?.Appointments));
    }
}

// ─── Inline request DTOs for simple one-off payloads ─────────────────────────

/// <summary>Payload for submitting a part request.</summary>
public class PartRequestDto
{
    [System.ComponentModel.DataAnnotations.Required]
    public string PartName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>Payload for submitting a review.</summary>
public class ReviewDto
{
    [System.ComponentModel.DataAnnotations.Range(1, 5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
