using Veltro.DTOs.Request.Part;
using Veltro.DTOs.Response.Part;
using Veltro.Helpers;

namespace Veltro.Services.Interfaces;

/// <summary>Parts management service contract.</summary>
public interface IPartService
{
    Task<PagedResult<PartResponseDto>> GetAllPartsAsync(int page, int pageSize);
    Task<PartResponseDto?> GetPartByIdAsync(Guid id);
    Task<PartResponseDto> CreatePartAsync(CreatePartDto dto);
    Task<PartResponseDto?> UpdatePartAsync(Guid id, UpdatePartDto dto);
    Task<bool> DeletePartAsync(Guid id);
    Task<PagedResult<PartResponseDto>> SearchPartsAsync(string query, int page, int pageSize);
    Task<PagedResult<PartResponseDto>> GetLowStockPartsAsync(int page, int pageSize);
}
