using Veltro.Models;

namespace Veltro.Repositories.Interfaces;

/// <summary>Customer-specific repository operations.</summary>
public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByUserIdAsync(Guid userId);
    Task<Customer?> GetWithDetailsAsync(Guid customerId);
    IQueryable<Customer> GetQueryable();

    Task AddVehicleAsync(Vehicle vehicle);
    Task<Vehicle?> GetVehicleByIdAsync(Guid vehicleId);
}
