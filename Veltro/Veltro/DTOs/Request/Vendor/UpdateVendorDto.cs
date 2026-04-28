using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Vendor;

/// <summary>Payload for updating an existing vendor.</summary>
public class UpdateVendorDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Address { get; set; }
}
