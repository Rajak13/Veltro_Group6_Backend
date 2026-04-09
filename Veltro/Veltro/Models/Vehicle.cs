using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Veltro.Models;

/// <summary>Customer-owned vehicle registered in the system.</summary>
public class Vehicle
{
    [Key]
    public Guid VehicleId { get; set; } = Guid.NewGuid();

    [ForeignKey(nameof(Customer))]
    public Guid CustomerId { get; set; }

    [Required]
    public string Make { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string? VIN { get; set; }

    public string? RegistrationNumber { get; set; }

    public int Mileage { get; set; }

    public DateTime? LastServiceDate { get; set; }

    // Navigation
    public Customer Customer { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
