using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Customer;

/// <summary>Payload for updating a customer profile.</summary>
public class UpdateCustomerDto
{
    [MaxLength(100)]
    public string? FullName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Address { get; set; }
}
