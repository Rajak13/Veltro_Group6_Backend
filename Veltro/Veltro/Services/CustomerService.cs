using Microsoft.EntityFrameworkCore;
using Veltro.Data;using Veltro.DTOs.Request.Customer;
using Veltro.DTOs.Response.Customer;
using Veltro.Helpers;
using Veltro.Models;
using Veltro.Repositories.Interfaces;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Manages customer profiles, vehicles, and history lookups.</summary>
public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IUserRepository _userRepo;
    private readonly AppDbContext _context;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository customerRepo, IUserRepository userRepo,
        AppDbContext context, ILogger<CustomerService> logger)
    {
        _customerRepo = customerRepo;
        _userRepo = userRepo;
        _context = context;
        _logger = logger;
    }

    /// <summary>Registers a new customer (and optional vehicle) by Staff.</summary>
    public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        try
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = PasswordHelper.Hash(dto.Password),
                Role = UserRole.Customer
            };
            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            var customer = new Customer
            {
                UserId = user.Id,
                Phone = dto.Phone,
                Address = dto.Address
            };
            await _customerRepo.AddAsync(customer);
            await _customerRepo.SaveChangesAsync();

            if (dto.Vehicle != null)
            {
                var vehicle = new Vehicle
                {
                    CustomerId = customer.CustomerId,
                    Make = dto.Vehicle.Make,
                    Model = dto.Vehicle.Model,
                    Year = dto.Vehicle.Year,
                    VIN = dto.Vehicle.VIN,
                    RegistrationNumber = dto.Vehicle.RegistrationNumber,
                    Mileage = dto.Vehicle.Mileage
                };
                await _customerRepo.AddVehicleAsync(vehicle);
                await _customerRepo.SaveChangesAsync();
            }

            return await GetCustomerByIdAsync(customer.CustomerId) ?? throw new Exception("Failed to retrieve created customer.");
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to create customer");
            throw;
        }
    }

    /// <summary>Returns a customer profile with vehicles by customer ID.</summary>
    public async Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid customerId)
    {
        try
        {
            var customer = await _customerRepo.GetWithDetailsAsync(customerId);
            return customer == null ? null : MapCustomerDto(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer {Id}", customerId);
            throw;
        }
    }

    /// <summary>Returns a customer profile by their linked User ID (used for JWT-authenticated calls).</summary>
    public async Task<CustomerResponseDto?> GetCustomerByUserIdAsync(Guid userId)
    {
        try
        {
            var customer = await _customerRepo.GetByUserIdAsync(userId);
            return customer == null ? null : MapCustomerDto(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer by userId {Id}", userId);
            throw;
        }
    }

    /// <summary>Returns a customer's full purchase and appointment history.</summary>
    public async Task<CustomerHistoryResponseDto?> GetCustomerHistoryAsync(Guid customerId)
    {
        try
        {
            var customer = await _customerRepo.GetWithDetailsAsync(customerId);
            if (customer == null) return null;

            return new CustomerHistoryResponseDto
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.User.FullName,
                Purchases = customer.SalesInvoices.Select(si => new PurchaseHistoryItemDto
                {
                    InvoiceId = si.InvoiceId,
                    SaleDate = si.SaleDate,
                    TotalAmount = si.TotalAmount,
                    DiscountApplied = si.DiscountApplied,
                    IsPaid = si.IsPaid
                }).ToList(),
                Appointments = customer.Appointments.Select(a => new AppointmentHistoryItemDto
                {
                    AppointmentId = a.AppointmentId,
                    ScheduledDate = a.ScheduledDate,
                    Status = a.Status.ToString(),
                    Notes = a.Notes
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get history for customer {Id}", customerId);
            throw;
        }
    }

    /// <summary>Updates a customer's profile fields.</summary>
    public async Task<CustomerResponseDto?> UpdateCustomerAsync(Guid userId, UpdateCustomerDto dto)
    {
        try
        {
            var customer = await _customerRepo.GetByUserIdAsync(userId);
            if (customer == null) return null;

            if (dto.Phone != null) customer.Phone = dto.Phone;
            if (dto.Address != null) customer.Address = dto.Address;

            if (dto.FullName != null)
            {
                customer.User.FullName = dto.FullName;
                _context.Users.Update(customer.User);
            }

            _customerRepo.Update(customer);
            await _customerRepo.SaveChangesAsync();
            return MapCustomerDto(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update customer for userId {Id}", userId);
            throw;
        }
    }

    /// <summary>Lists customers with optional free-text search across name AND phone.</summary>
    public async Task<PagedResult<CustomerResponseDto>> GetCustomersAsync(
        string? search, int page, int pageSize)
    {
        try
        {
            var q = _customerRepo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(c =>
                    EF.Functions.ILike(c.User.FullName, $"%{s}%") ||
                    (c.Phone != null && EF.Functions.ILike(c.Phone, $"%{s}%")));
            }

            var projected = q.Select(c => new CustomerResponseDto
            {
                CustomerId   = c.CustomerId,
                UserId       = c.UserId,
                FullName     = c.User.FullName,
                Email        = c.User.Email,
                Phone        = c.Phone,
                Address      = c.Address,
                CreditBalance = c.CreditBalance,
                Vehicles     = c.Vehicles.Select(v => new VehicleSummaryDto
                {
                    VehicleId          = v.VehicleId,
                    Make               = v.Make,
                    Model              = v.Model,
                    Year               = v.Year,
                    RegistrationNumber = v.RegistrationNumber
                }).ToList()
            });

            return await PaginationHelper.PaginateAsync(projected, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCustomersAsync failed for search '{Search}'", search);
            throw;
        }
    }

    /// <summary>Searches customers by name, phone, ID, or vehicle registration.</summary>
    public async Task<PagedResult<CustomerResponseDto>> SearchCustomersAsync(
        string query, string type, int page, int pageSize)
    {
        try
        {
            var q = _customerRepo.GetQueryable();

            q = type.ToLower() switch
            {
                "phone"   => q.Where(c => c.Phone != null && EF.Functions.ILike(c.Phone, $"%{query}%")),
                "id"      => q.Where(c => c.CustomerId.ToString() == query),
                "vehicle" => q.Where(c => c.Vehicles.Any(v =>
                                v.RegistrationNumber != null && EF.Functions.ILike(v.RegistrationNumber, $"%{query}%"))),
                _         => q.Where(c => EF.Functions.ILike(c.User.FullName, $"%{query}%"))
            };

            var projected = q.Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                UserId = c.UserId,
                FullName = c.User.FullName,
                Email = c.User.Email,
                Phone = c.Phone,
                Address = c.Address,
                CreditBalance = c.CreditBalance,
                Vehicles = c.Vehicles.Select(v => new VehicleSummaryDto
                {
                    VehicleId = v.VehicleId,
                    Make = v.Make,
                    Model = v.Model,
                    Year = v.Year,
                    RegistrationNumber = v.RegistrationNumber
                }).ToList()
            });

            return await PaginationHelper.PaginateAsync(projected, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer search failed for query '{Query}'", query);
            throw;
        }
    }

    /// <summary>Adds a vehicle to a customer's profile.</summary>
    public async Task AddVehicleAsync(Guid userId, CreateVehicleDto dto)
    {
        try
        {
            var customer = await _customerRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Customer not found.");

            var vehicle = new Vehicle
            {
                CustomerId = customer.CustomerId,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                VIN = dto.VIN,
                RegistrationNumber = dto.RegistrationNumber,
                Mileage = dto.Mileage
            };
            await _customerRepo.AddVehicleAsync(vehicle);
            await _customerRepo.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Failed to add vehicle for userId {Id}", userId);
            throw;
        }
    }

    /// <summary>Updates an existing vehicle's details.</summary>
    public async Task UpdateVehicleAsync(Guid vehicleId, CreateVehicleDto dto)
    {
        try
        {
            var vehicle = await _customerRepo.GetVehicleByIdAsync(vehicleId)
                ?? throw new KeyNotFoundException("Vehicle not found.");

            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;
            vehicle.VIN = dto.VIN;
            vehicle.RegistrationNumber = dto.RegistrationNumber;
            vehicle.Mileage = dto.Mileage;

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Failed to update vehicle {Id}", vehicleId);
            throw;
        }
    }

    private static CustomerResponseDto MapCustomerDto(Customer c) => new()
    {
        CustomerId = c.CustomerId,
        UserId = c.UserId,
        FullName = c.User?.FullName ?? string.Empty,
        Email = c.User?.Email ?? string.Empty,
        Phone = c.Phone,
        Address = c.Address,
        CreditBalance = c.CreditBalance,
        Vehicles = c.Vehicles.Select(v => new VehicleSummaryDto
        {
            VehicleId = v.VehicleId,
            Make = v.Make,
            Model = v.Model,
            Year = v.Year,
            RegistrationNumber = v.RegistrationNumber
        }).ToList()
    };
}
