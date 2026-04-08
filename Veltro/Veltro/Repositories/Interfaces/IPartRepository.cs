using Veltro.Models;

namespace Veltro.Repositories.Interfaces;

/// <summary>Part-specific repository operations.</summary>
public interface IPartRepository : IRepository<Part>
{
    /// <summary>Returns all parts where StockQuantity is below LowStockThreshold.</summary>
    Task<IEnumerable<Part>> GetLowStockPartsAsync();

    /// <summary>Returns a queryable for pagination support.</summary>
    IQueryable<Part> GetQueryable();
}
