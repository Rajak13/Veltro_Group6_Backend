using Veltro.DTOs.Request.Auth;
using Veltro.DTOs.Response.Auth;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Repositories.Interfaces;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Handles user authentication and customer self-registration.</summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly JwtHelper _jwtHelper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepo, ICustomerRepository customerRepo,
        JwtHelper jwtHelper, ILogger<AuthService> logger)
    {
        _userRepo = userRepo;
        _customerRepo = customerRepo;
        _jwtHelper = jwtHelper;
        _logger = logger;
    }

    /// <summary>Validates credentials and returns a JWT token on success.</summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated.");

            if (!PasswordHelper.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Login failed for {Email}", dto.Email);
            throw;
        }
    }

    /// <summary>Registers a new Customer account.</summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        try
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = PasswordHelper.Hash(dto.Password),
                Role = UserRole.Customer
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var customer = new Customer
            {
                UserId = user.Id,
                Phone = dto.Phone,
                Address = dto.Address
            };

            await _customerRepo.AddAsync(customer);
            await _customerRepo.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Registration failed for {Email}", dto.Email);
            throw;
        }
    }
}
