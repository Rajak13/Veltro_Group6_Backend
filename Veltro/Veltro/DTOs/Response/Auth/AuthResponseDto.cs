namespace Veltro.DTOs.Response.Auth;

/// <summary>Returned after a successful login containing the JWT token and user info.</summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
