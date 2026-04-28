using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>Customer repository with profile and vehicle query support.</summary>
public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    /// <summary>Finds a customer by their linked User ID.</summary>
    public async Task<Customer?> GetByUserIdAsync(Guid userId) =>
        await _context.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    /// <summary>Returns a customer with all related data for profile/history views.</summary>
    public async Task<Customer?> GetWithDetailsAsync(Guid customerId) =>
        await _context.Customers
            .Include(c => c.User)
            .Include(c => c.Vehicles)
            .Include(c => c.SalesInvoices).ThenInclude(si => si.Items).ThenInclude(i => i.Part)
            .Include(c => c.Appointments)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

    /// <summary>Returns a queryable for search and pagination.</summary>
    public IQueryable<Customer> GetQueryable() =>
        _context.Customers.AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.Vehicles);

    public async Task AddVehicleAsync(Vehicle vehicle) =>
        await _context.Vehicles.AddAsync(vehicle);

    public async Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId) =>
        await _context.Vehicles.FindAsync(vehicleId);
}
