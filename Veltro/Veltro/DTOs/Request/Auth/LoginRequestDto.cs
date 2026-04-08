using System.ComponentModel.DataAnnotations;

namespace Veltro.DTOs.Request.Auth;

/// <summary>Payload for user login.</summary>
public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
