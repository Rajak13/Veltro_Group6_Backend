namespace Veltro.DTOs.Response.Vendor;

/// <summary>Vendor details returned in list and detail responses.</summary>
public class VendorResponseDto
{
    public Guid VendorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}
