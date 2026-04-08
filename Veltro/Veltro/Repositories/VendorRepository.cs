using Veltro.Data;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>Vendor repository implementation.</summary>
public class VendorRepository : BaseRepository<Vendor>, IVendorRepository
{
    public VendorRepository(AppDbContext context) : base(context) { }

    /// <summary>Returns a queryable for pagination and filtering.</summary>
    public IQueryable<Vendor> GetQueryable() =>
        _context.Vendors.AsQueryable();
}
