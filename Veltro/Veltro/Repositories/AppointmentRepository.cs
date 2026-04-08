using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Models;
using Veltro.Repositories.Interfaces;

namespace Veltro.Repositories;

/// <summary>Appointment repository with customer-scoped query support.</summary>
public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }

    /// <summary>Returns all appointments for a specific customer.</summary>
    public async Task<IEnumerable<Appointment>> GetByCustomerIdAsync(Guid customerId) =>
        await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Vehicle)
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();

    /// <summary>Returns a queryable for pagination support.</summary>
    public IQueryable<Appointment> GetQueryable() =>
        _context.Appointments.AsNoTracking().Include(a => a.Vehicle);
}
