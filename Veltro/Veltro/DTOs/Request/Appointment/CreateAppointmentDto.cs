using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Appointment;

/// <summary>Payload for booking a service appointment.</summary>
public class CreateAppointmentDto
{
    [Required]
    public Guid VehicleId { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    public string? ServiceType { get; set; }

    public string? Notes { get; set; }
}
