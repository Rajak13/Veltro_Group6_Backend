using Veltro.Models;

namespace Veltro.Repositories.Interfaces;

/// <summary>Appointment-specific repository operations.</summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByCustomerIdAsync(Guid customerId);
    IQueryable<Appointment> GetQueryable();
}
