using Veltro.DTOs.Request.Auth;
using Veltro.DTOs.Response.Auth;

namespace Veltro.Services.Interfaces;

/// <summary>Authentication service contract.</summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
}
