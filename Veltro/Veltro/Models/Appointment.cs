using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Service appointment booked by a Customer for a Vehicle.</summary>
public class Appointment
{
    [Key]
    public Guid AppointmentId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; set; }

    [ForeignKey(nameof(Vehicle))]
    public Guid VehicleId { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string? ServiceType { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Customer Customer { get; set; } = null!;
    public Vehicle Vehicle { get; set; } = null!;
}
