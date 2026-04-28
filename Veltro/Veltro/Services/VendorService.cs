using AutoMapper;
using Veltro.DTOs.Request.Vendor;
using Veltro.DTOs.Response.Vendor;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Repositories.Interfaces;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Manages vendor records for the parts supply chain.</summary>
public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<VendorService> _logger;

    public VendorService(IVendorRepository vendorRepo, IMapper mapper, ILogger<VendorService> logger)
    {
        _vendorRepo = vendorRepo;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Returns a paginated list of all vendors.</summary>
    public async Task<PagedResult<VendorResponseDto>> GetAllVendorsAsync(int page, int pageSize)
    {
        try
        {
            var query = _vendorRepo.GetQueryable()
                .Select(v => new VendorResponseDto
                {
                    VendorId = v.VendorId,
                    Name = v.Name,
                    ContactPerson = v.ContactPerson,
                    Phone = v.Phone,
                    Email = v.Email,
                    Address = v.Address,
                    IsActive = v.IsActive
                });
            return await PaginationHelper.PaginateAsync(query, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve vendors");
            throw;
        }
    }

    /// <summary>Returns a single vendor by ID.</summary>
    public async Task<VendorResponseDto?> GetVendorByIdAsync(Guid id)
    {
        try
        {
            var vendor = await _vendorRepo.GetByIdAsync(id);
            return vendor == null ? null : _mapper.Map<VendorResponseDto>(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve vendor {Id}", id);
            throw;
        }
    }

    /// <summary>Creates a new vendor.</summary>
    public async Task<VendorResponseDto> CreateVendorAsync(CreateVendorDto dto)
    {
        try
        {
            var vendor = _mapper.Map<Vendor>(dto);
            await _vendorRepo.AddAsync(vendor);
            await _vendorRepo.SaveChangesAsync();
            return _mapper.Map<VendorResponseDto>(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create vendor");
            throw;
        }
    }

    /// <summary>Updates an existing vendor's details.</summary>
    public async Task<VendorResponseDto?> UpdateVendorAsync(Guid id, UpdateVendorDto dto)
    {
        try
        {
            var vendor = await _vendorRepo.GetByIdAsync(id);
            if (vendor == null) return null;

            if (dto.Name != null) vendor.Name = dto.Name;
            if (dto.ContactPerson != null) vendor.ContactPerson = dto.ContactPerson;
            if (dto.Phone != null) vendor.Phone = dto.Phone;
            if (dto.Email != null) vendor.Email = dto.Email;
            if (dto.Address != null) vendor.Address = dto.Address;

            _vendorRepo.Update(vendor);
            await _vendorRepo.SaveChangesAsync();
            return _mapper.Map<VendorResponseDto>(vendor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update vendor {Id}", id);
            throw;
        }
    }

    /// <summary>Soft-deletes a vendor by setting IsActive to false.</summary>
    public async Task<bool> DeleteVendorAsync(Guid id)
    {
        try
        {
            var vendor = await _vendorRepo.GetByIdAsync(id);
            if (vendor == null) return false;

            vendor.IsActive = false;
            _vendorRepo.Update(vendor);
            return await _vendorRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete vendor {Id}", id);
            throw;
        }
    }
}
