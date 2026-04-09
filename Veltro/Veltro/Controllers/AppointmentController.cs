using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Veltro.Controllers;

/// <summary>
/// Appointment routes are handled inside CustomerController via route overrides.
/// This stub exists to satisfy the project structure requirement.
/// </summary>
[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentController : ControllerBase
{
    // All appointment logic lives in CustomerController using [HttpPost("/api/appointments")] etc.
    // This controller can be extended by team members for admin appointment management.
}
