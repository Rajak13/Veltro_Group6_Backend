using Veltro.DTOs.Request.Customer;
using Veltro.DTOs.Response.Customer;
using Veltro.Helpers;

namespace Veltro.Services.Interfaces;

/// <summary>Customer management service contract.</summary>
public interface ICustomerService
{
    Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
    Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid customerId);
    Task<CustomerResponseDto?> GetCustomerByUserIdAsync(Guid userId);
    Task<CustomerHistoryResponseDto?> GetCustomerHistoryAsync(Guid customerId);
    Task<CustomerResponseDto?> UpdateCustomerAsync(Guid userId, UpdateCustomerDto dto);
    Task<PagedResult<CustomerResponseDto>> GetCustomersAsync(string? search, int page, int pageSize);
    Task<PagedResult<CustomerResponseDto>> SearchCustomersAsync(string query, string type, int page, int pageSize);
    Task AddVehicleAsync(Guid userId, CreateVehicleDto dto);
    Task UpdateVehicleAsync(Guid vehicleId, CreateVehicleDto dto);
}
