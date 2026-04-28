using Veltro.DTOs.Request.Vendor;
using Veltro.DTOs.Response.Vendor;
using Veltro.Helpers;

namespace Veltro.Services.Interfaces;

/// <summary>Vendor management service contract.</summary>
public interface IVendorService
{
    Task<PagedResult<VendorResponseDto>> GetAllVendorsAsync(int page, int pageSize);
    Task<VendorResponseDto?> GetVendorByIdAsync(Guid id);
    Task<VendorResponseDto> CreateVendorAsync(CreateVendorDto dto);
    Task<VendorResponseDto?> UpdateVendorAsync(Guid id, UpdateVendorDto dto);
    Task<bool> DeleteVendorAsync(Guid id);
}
