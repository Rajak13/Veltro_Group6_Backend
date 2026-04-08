using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Vendor;

/// <summary>Payload for creating a new vendor.</summary>
public class CreateVendorDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Address { get; set; }
}
