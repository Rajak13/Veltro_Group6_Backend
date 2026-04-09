namespace Veltro.DTOs.Response.Customer;

/// <summary>Customer profile details returned in responses.</summary>
public class CustomerResponseDto
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal CreditBalance { get; set; }
    public List<VehicleSummaryDto> Vehicles { get; set; } = new();
}

/// <summary>Minimal vehicle info for embedding in customer responses.</summary>
public class VehicleSummaryDto
{
    public Guid VehicleId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? RegistrationNumber { get; set; }
}
