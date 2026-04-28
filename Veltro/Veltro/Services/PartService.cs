using AutoMapper;
using Veltro.DTOs.Request.Part;
using Veltro.DTOs.Response.Part;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Repositories.Interfaces;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Manages vehicle parts inventory including low-stock notifications.</summary>
public class PartService : IPartService
{
    private readonly IPartRepository _partRepo;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<PartService> _logger;

    public PartService(IPartRepository partRepo, IMapper mapper,
        INotificationService notificationService, IUserRepository userRepo,
        ILogger<PartService> logger)
    {
        _partRepo = partRepo;
        _mapper = mapper;
        _notificationService = notificationService;
        _userRepo = userRepo;
        _logger = logger;
    }

    /// <summary>Returns a paginated list of all parts with vendor info.</summary>
    public async Task<PagedResult<PartResponseDto>> GetAllPartsAsync(int page, int pageSize)
    {
        try
        {
            var query = _partRepo.GetQueryable()
                .Select(p => new PartResponseDto
                {
                    PartId = p.PartId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    VendorId = p.VendorId,
                    VendorName = p.Vendor.Name,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                });

            return await PaginationHelper.PaginateAsync(query, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve parts list");
            throw;
        }
    }

    /// <summary>Returns a single part by ID.</summary>
    public async Task<PartResponseDto?> GetPartByIdAsync(Guid id)
    {
        try
        {
            var part = await _partRepo.GetByIdAsync(id);
            return part == null ? null : _mapper.Map<PartResponseDto>(part);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve part {Id}", id);
            throw;
        }
    }

    /// <summary>Creates a new part and triggers low-stock notification if initial stock is below threshold.</summary>
    public async Task<PartResponseDto> CreatePartAsync(CreatePartDto dto)
    {
        try
        {
            var part = _mapper.Map<Part>(dto);
            part.CreatedAt = DateTime.UtcNow;
            part.UpdatedAt = DateTime.UtcNow;

            await _partRepo.AddAsync(part);
            await _partRepo.SaveChangesAsync();

            await CheckAndNotifyLowStockAsync(part);

            return _mapper.Map<PartResponseDto>(part);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create part");
            throw;
        }
    }

    /// <summary>Updates an existing part's details.</summary>
    public async Task<PartResponseDto?> UpdatePartAsync(Guid id, UpdatePartDto dto)
    {
        try
        {
            var part = await _partRepo.GetByIdAsync(id);
            if (part == null) return null;

            if (dto.Name != null) part.Name = dto.Name;
            if (dto.Description != null) part.Description = dto.Description;
            if (dto.Price.HasValue) part.Price = dto.Price.Value;
            if (dto.StockQuantity.HasValue) part.StockQuantity = dto.StockQuantity.Value;
            if (dto.LowStockThreshold.HasValue) part.LowStockThreshold = dto.LowStockThreshold.Value;
            if (dto.VendorId.HasValue) part.VendorId = dto.VendorId.Value;
            part.UpdatedAt = DateTime.UtcNow;

            _partRepo.Update(part);
            await _partRepo.SaveChangesAsync();

            await CheckAndNotifyLowStockAsync(part);

            return _mapper.Map<PartResponseDto>(part);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update part {Id}", id);
            throw;
        }
    }

    /// <summary>Deletes a part by ID.</summary>
    public async Task<bool> DeletePartAsync(Guid id)
    {
        try
        {
            var part = await _partRepo.GetByIdAsync(id);
            if (part == null) return false;

            _partRepo.Delete(part);
            return await _partRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete part {Id}", id);
            throw;
        }
    }

    /// <summary>Sends a low-stock notification to all Admin users if stock is below threshold.</summary>
    public async Task CheckAndNotifyLowStockAsync(Part part)
    {
        if (part.StockQuantity >= part.LowStockThreshold) return;

        var admins = await _userRepo.GetByRoleAsync(Models.UserRole.Admin);
        foreach (var admin in admins)
        {
            await _notificationService.CreateNotificationAsync(
                admin.Id,
                $"Low stock alert: '{part.Name}' has only {part.StockQuantity} units remaining (threshold: {part.LowStockThreshold}).",
                Models.NotificationType.LowStock
            );
        }
    }
}
