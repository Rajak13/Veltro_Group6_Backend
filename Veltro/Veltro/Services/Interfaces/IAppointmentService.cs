using Veltro.DTOs.Request.Appointment;
using Veltro.Models;

namespace Veltro.Services.Interfaces;

/// <summary>Appointment management service contract.</summary>
public interface IAppointmentService
{
    Task<Appointment> CreateAppointmentAsync(Guid customerId, CreateAppointmentDto dto);
    Task<IEnumerable<Appointment>> GetCustomerAppointmentsAsync(Guid customerId);
    Task<bool> CancelAppointmentAsync(Guid appointmentId, Guid customerId);
}
