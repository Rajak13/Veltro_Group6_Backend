using Veltro.Models;

namespace Veltro.Repositories.Interfaces;

/// <summary>Vendor-specific repository operations.</summary>
public interface IVendorRepository : IRepository<Vendor>
{
    IQueryable<Vendor> GetQueryable();
}
