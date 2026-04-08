using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>Part repository with stock-level query support.</summary>
public class PartRepository : BaseRepository<Part>, IPartRepository
{
    public PartRepository(AppDbContext context) : base(context) { }

    /// <summary>Returns all parts where current stock is below the configured threshold.</summary>
    public async Task<IEnumerable<Part>> GetLowStockPartsAsync() =>
        await _context.Parts
            .AsNoTracking()
            .Where(p => p.StockQuantity < p.LowStockThreshold)
            .ToListAsync();

    /// <summary>Returns a queryable for pagination and filtering.</summary>
    public IQueryable<Part> GetQueryable() =>
        _context.Parts.AsNoTracking().Include(p => p.Vendor);
}
