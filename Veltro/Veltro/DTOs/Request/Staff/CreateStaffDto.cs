using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Staff;

/// <summary>Payload for Admin registering a new staff member.</summary>
public class CreateStaffDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? Position { get; set; }
}
