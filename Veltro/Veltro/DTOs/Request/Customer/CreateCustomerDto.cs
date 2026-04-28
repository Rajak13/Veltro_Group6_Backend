using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Customer;

/// <summary>Payload for Staff registering a new customer with an optional vehicle.</summary>
public class CreateCustomerDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Address { get; set; }

    /// <summary>Optional vehicle to register alongside the customer.</summary>
    public CreateVehicleDto? Vehicle { get; set; }
}

/// <summary>Vehicle details for inline registration.</summary>
public class CreateVehicleDto
{
    [Required]
    public string Make { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string? VIN { get; set; }

    public string? RegistrationNumber { get; set; }

    public int Mileage { get; set; }
}
