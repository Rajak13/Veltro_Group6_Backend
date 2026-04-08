using Veltro.DTOs.Request.Appointment;
using Veltro.Models;
using Veltro.Repositories.Interfaces;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Manages customer service appointments.</summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository appointmentRepo,
        ICustomerRepository customerRepo, ILogger<AppointmentService> logger)
    {
        _appointmentRepo = appointmentRepo;
        _customerRepo = customerRepo;
        _logger = logger;
    }

    /// <summary>Books a new appointment for a customer's vehicle.</summary>
    public async Task<Appointment> CreateAppointmentAsync(Guid customerId, CreateAppointmentDto dto)
    {
        try
        {
            var appointment = new Appointment
            {
                CustomerId = customerId,
                VehicleId = dto.VehicleId,
                ScheduledDate = dto.ScheduledDate,
                Notes = dto.Notes,
                Status = AppointmentStatus.Pending
            };
            await _appointmentRepo.AddAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();
            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create appointment for customer {Id}", customerId);
            throw;
        }
    }

    /// <summary>Returns all appointments for a specific customer.</summary>
    public async Task<IEnumerable<Appointment>> GetCustomerAppointmentsAsync(Guid customerId)
    {
        try
        {
            return await _appointmentRepo.GetByCustomerIdAsync(customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get appointments for customer {Id}", customerId);
            throw;
        }
    }

    /// <summary>Cancels an appointment — only the owning customer may cancel.</summary>
    public async Task<bool> CancelAppointmentAsync(Guid appointmentId, Guid customerId)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId);
            if (appointment == null || appointment.CustomerId != customerId)
                return false;

            appointment.Status = AppointmentStatus.Cancelled;
            _appointmentRepo.Update(appointment);
            return await _appointmentRepo.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel appointment {Id}", appointmentId);
            throw;
        }
    }
}
