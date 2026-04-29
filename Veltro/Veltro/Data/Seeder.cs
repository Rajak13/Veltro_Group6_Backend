using Veltro.Models;

namespace Veltro.Data;

/// <summary>Seeds the database with comprehensive realistic test data simulating an active business.</summary>
public static class Seeder
{
    /// <summary>Creates realistic test data with multiple users, transactions, and ongoing business activity.</summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if seeding has already been done
        if (context.Users.Any(u => u.Role == UserRole.Admin))
            return;

        // ═══════════════════════════════════════════════════════════════════════
        // USERS & ROLES (10 users total)
        // ═══════════════════════════════════════════════════════════════════════

        // Admin
        var adminUserId = Guid.NewGuid();
        var admin = new User
        {
            Id = adminUserId,
            FullName = "System Admin",
            Email = "admin@veltro.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-6)
        };
        context.Users.Add(admin);

        // Staff Members (3)
        var staffIds = new List<Guid>();
        var staffUserIds = new List<Guid>();
        
        var staffData = new[]
        {
            ("Rajesh Kumar", "staff@gmail.com", "Service Technician"),
            ("Sita Sharma", "sita.sharma@veltro.com", "Senior Mechanic"),
            ("Prakash Rai", "prakash.rai@veltro.com", "Parts Specialist")
        };

        foreach (var (name, email, position) in staffData)
        {
            var userId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            
            context.Users.Add(new User
            {
                Id = userId,
                FullName = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123@"),
                Role = UserRole.Staff,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            });

            context.Staff.Add(new Staff
            {
                StaffId = staffId,
                UserId = userId,
                Position = position
            });

            staffUserIds.Add(userId);
            staffIds.Add(staffId);
        }

        // Customers (6 - mix of active and new)
        var customerIds = new List<Guid>();
        var customerUserIds = new List<Guid>();
        
        var customerData = new[]
        {
            ("Ramesh Thapa", "customer1@gmail.com", "9841234567", "Thamel, Kathmandu", -60),
            ("Anita Gurung", "anita.gurung@gmail.com", "9851234567", "Patan Dhoka, Lalitpur", -45),
            ("Bikash Rai", "bikash.rai@gmail.com", "9861234567", "Boudha, Kathmandu", -30),
            ("Sunita Magar", "sunita.magar@gmail.com", "9871234567", "Baneshwor, Kathmandu", -15),
            ("Dipak Shrestha", "dipak.shrestha@gmail.com", "9881234567", "Bhaktapur Durbar", -7),
            ("Maya Tamang", "maya.tamang@gmail.com", "9891234567", "Kirtipur, Kathmandu", -2)
        };

        foreach (var (name, email, phone, address, daysAgo) in customerData)
        {
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            
            context.Users.Add(new User
            {
                Id = userId,
                FullName = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123@"),
                Role = UserRole.Customer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(daysAgo)
            });

            context.Customers.Add(new Customer
            {
                CustomerId = customerId,
                UserId = userId,
                Phone = phone,
                Address = address
            });

            customerUserIds.Add(userId);
            customerIds.Add(customerId);
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // VENDORS (5 vendors)
        // ═══════════════════════════════════════════════════════════════════════

        var vendorIds = new List<Guid>();
        var vendorData = new[]
        {
            ("AutoParts Nepal Pvt. Ltd.", "Mohan Shrestha", "contact@autopartsnepal.com", "01-4567890", "New Road, Kathmandu"),
            ("Himalayan Motors Supply", "Krishna Tamang", "info@himalayanmotors.com", "01-5678901", "Balaju, Kathmandu"),
            ("Everest Auto Parts", "Lakpa Sherpa", "sales@everestauto.com", "01-6789012", "Kalanki, Kathmandu"),
            ("Nepal Auto Traders", "Ramesh Gurung", "info@nepalauto.com", "01-7890123", "Koteshwor, Kathmandu"),
            ("Mountain View Parts Co.", "Sanjay Thapa", "sales@mountainview.com", "01-8901234", "Chabahil, Kathmandu")
        };

        foreach (var (name, contact, email, phone, address) in vendorData)
        {
            var vendorId = Guid.NewGuid();
            context.Vendors.Add(new Vendor
            {
                VendorId = vendorId,
                Name = name,
                ContactPerson = contact,
                Email = email,
                Phone = phone,
                Address = address,
                IsActive = true
            });
            vendorIds.Add(vendorId);
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // PARTS INVENTORY (20 parts - realistic auto parts)
        // ═══════════════════════════════════════════════════════════════════════

        var partIds = new List<Guid>();
        var partData = new[]
        {
            ("Engine Oil 5W-30", "Synthetic engine oil for modern vehicles", 1200.00m, 45, 20, 0),
            ("Engine Oil 10W-40", "Semi-synthetic engine oil", 950.00m, 38, 20, 0),
            ("Brake Pads (Front)", "High-performance ceramic brake pads", 3500.00m, 28, 15, 0),
            ("Brake Pads (Rear)", "Standard brake pads for rear wheels", 2800.00m, 22, 15, 1),
            ("Air Filter", "High-efficiency air filter", 850.00m, 62, 25, 1),
            ("Cabin Air Filter", "HEPA cabin air filter", 1100.00m, 35, 20, 2),
            ("Spark Plugs (Set of 4)", "Iridium spark plugs", 2400.00m, 18, 10, 1),
            ("Battery 12V 65Ah", "Maintenance-free car battery", 8500.00m, 12, 8, 2),
            ("Wiper Blades (Pair)", "All-season wiper blades", 650.00m, 8, 15, 2),
            ("Transmission Fluid", "Automatic transmission fluid ATF", 1800.00m, 35, 20, 0),
            ("Coolant (5L)", "Engine coolant antifreeze", 1500.00m, 22, 15, 1),
            ("Brake Fluid DOT 4", "High-performance brake fluid", 450.00m, 40, 25, 3),
            ("Power Steering Fluid", "Hydraulic power steering fluid", 550.00m, 28, 20, 3),
            ("Timing Belt", "Rubber timing belt", 4500.00m, 15, 10, 0),
            ("Serpentine Belt", "Multi-ribbed serpentine belt", 1200.00m, 20, 12, 1),
            ("Headlight Bulb H7", "Halogen headlight bulb", 350.00m, 50, 30, 4),
            ("Tail Light Bulb", "Standard tail light bulb", 150.00m, 65, 40, 4),
            ("Fuel Filter", "Inline fuel filter", 800.00m, 32, 20, 1),
            ("Oil Filter", "Spin-on oil filter", 450.00m, 55, 30, 0),
            ("Radiator Cap", "Pressure radiator cap", 250.00m, 42, 25, 2)
        };

        foreach (var (name, desc, price, stock, threshold, vendorIdx) in partData)
        {
            var partId = Guid.NewGuid();
            context.Parts.Add(new Part
            {
                PartId = partId,
                Name = name,
                Description = desc,
                Price = price,
                StockQuantity = stock,
                LowStockThreshold = threshold,
                VendorId = vendorIds[vendorIdx],
                CreatedAt = DateTime.UtcNow.AddMonths(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30))
            });
            partIds.Add(partId);
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // VEHICLES (10 vehicles across customers)
        // ═══════════════════════════════════════════════════════════════════════

        var vehicleIds = new List<Guid>();
        var vehicleData = new[]
        {
            (0, "Toyota", "Corolla", 2019, "BA 12 PA 1234", 45000),
            (0, "Honda", "City", 2021, "BA 15 PA 5678", 28000),
            (1, "Hyundai", "Creta", 2020, "BA 18 PA 9012", 35000),
            (1, "Maruti", "Swift", 2022, "BA 20 PA 3456", 15000),
            (2, "Suzuki", "Swift", 2018, "BA 10 PA 7890", 62000),
            (3, "Toyota", "Yaris", 2021, "BA 16 PA 2345", 22000),
            (3, "Nissan", "Kicks", 2020, "BA 17 PA 6789", 38000),
            (4, "Hyundai", "i20", 2023, "BA 22 PA 1111", 8000),
            (5, "Honda", "Jazz", 2019, "BA 14 PA 5555", 48000),
            (5, "Toyota", "Fortuner", 2022, "BA 21 PA 9999", 12000)
        };

        foreach (var (custIdx, make, model, year, reg, mileage) in vehicleData)
        {
            var vehicleId = Guid.NewGuid();
            context.Vehicles.Add(new Vehicle
            {
                VehicleId = vehicleId,
                CustomerId = customerIds[custIdx],
                Make = make,
                Model = model,
                Year = year,
                RegistrationNumber = reg,
                Mileage = mileage
            });
            vehicleIds.Add(vehicleId);
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // APPOINTMENTS (15 appointments - past, present, future)
        // ═══════════════════════════════════════════════════════════════════════

        var appointmentData = new[]
        {
            (0, 0, "Oil Change", -15, AppointmentStatus.Completed, "Regular maintenance"),
            (0, 1, "Brake Inspection", 3, AppointmentStatus.Pending, "Customer reported squeaking"),
            (1, 2, "General Service", -8, AppointmentStatus.Completed, "30,000 km service"),
            (1, 3, "Tire Rotation", 5, AppointmentStatus.Pending, "Scheduled maintenance"),
            (2, 4, "Engine Check", -3, AppointmentStatus.Completed, "Check engine light on"),
            (3, 5, "Oil Change", 7, AppointmentStatus.Pending, "Regular service"),
            (3, 6, "Brake Service", -20, AppointmentStatus.Completed, "Brake pad replacement"),
            (4, 7, "General Inspection", 2, AppointmentStatus.Pending, "Pre-purchase inspection"),
            (5, 8, "AC Service", -5, AppointmentStatus.Completed, "AC not cooling properly"),
            (5, 9, "Transmission Service", 10, AppointmentStatus.Pending, "Scheduled maintenance"),
            (0, 0, "Tire Change", -30, AppointmentStatus.Completed, "Winter tires"),
            (1, 2, "Battery Check", 1, AppointmentStatus.Pending, "Battery warning light"),
            (2, 4, "Alignment", -12, AppointmentStatus.Completed, "Car pulling to left"),
            (4, 7, "Oil Change", 14, AppointmentStatus.Pending, "Regular maintenance"),
            (3, 5, "Brake Fluid Change", -25, AppointmentStatus.Completed, "Scheduled service")
        };

        foreach (var (custIdx, vehIdx, service, daysOffset, status, notes) in appointmentData)
        {
            context.Appointments.Add(new Appointment
            {
                AppointmentId = Guid.NewGuid(),
                CustomerId = customerIds[custIdx],
                VehicleId = vehicleIds[vehIdx],
                ServiceType = service,
                ScheduledDate = DateTime.UtcNow.AddDays(daysOffset),
                Status = status,
                Notes = notes,
                CreatedAt = DateTime.UtcNow.AddDays(daysOffset - 2)
            });
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // PURCHASE INVOICES (8 invoices from different vendors)
        // ═══════════════════════════════════════════════════════════════════════

        var purchaseInvoiceData = new[]
        {
            (0, -45, 45000.00m, "Bulk oil and filters order"),
            (1, -38, 28000.00m, "Brake parts shipment"),
            (2, -30, 52000.00m, "Batteries and electrical"),
            (0, -22, 18500.00m, "Oil and fluids restock"),
            (3, -15, 35000.00m, "Belts and hoses"),
            (4, -10, 12000.00m, "Light bulbs and accessories"),
            (1, -5, 22000.00m, "Brake pads and rotors"),
            (2, -2, 15000.00m, "Batteries restock")
        };

        foreach (var (vendorIdx, daysAgo, amount, notes) in purchaseInvoiceData)
        {
            context.PurchaseInvoices.Add(new PurchaseInvoice
            {
                InvoiceId = Guid.NewGuid(),
                VendorId = vendorIds[vendorIdx],
                PurchaseDate = DateTime.UtcNow.AddDays(daysAgo),
                TotalAmount = amount,
                Notes = notes
            });
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // SALES INVOICES (12 invoices - realistic transaction history)
        // ═══════════════════════════════════════════════════════════════════════

        var salesInvoiceData = new[]
        {
            (0, 0, -40, new[] { (0, 2), (18, 1) }, true),
            (1, 1, -35, new[] { (2, 1), (8, 1) }, true),
            (2, 0, -28, new[] { (1, 2), (4, 2) }, true),
            (0, 1, -20, new[] { (7, 1), (6, 2) }, true),
            (3, 2, -15, new[] { (3, 1), (10, 2) }, true),
            (1, 0, -12, new[] { (0, 1), (18, 1) }, true),
            (4, 1, -8, new[] { (9, 2), (11, 1) }, true),
            (2, 2, -5, new[] { (4, 1), (17, 1) }, true),
            (5, 0, -3, new[] { (0, 1), (8, 1) }, false),
            (0, 1, -2, new[] { (15, 4), (16, 2) }, false),
            (3, 2, -1, new[] { (2, 1), (3, 1) }, false),
            (1, 0, 0, new[] { (13, 1), (14, 1) }, false)
        };

        foreach (var (custIdx, staffIdx, daysAgo, items, isPaid) in salesInvoiceData)
        {
            var invoiceId = Guid.NewGuid();
            var saleDate = DateTime.UtcNow.AddDays(daysAgo);
            
            decimal subtotal = 0;
            foreach (var (partIdx, qty) in items)
            {
                var part = await context.Parts.FindAsync(partIds[partIdx]);
                subtotal += (part?.Price ?? 0) * qty;
            }

            var discount = subtotal >= 5000 ? subtotal * 0.1m : 0;
            var total = subtotal - discount;

            var invoice = new SalesInvoice
            {
                InvoiceId = invoiceId,
                CustomerId = customerIds[custIdx],
                StaffId = staffIds[staffIdx],
                SaleDate = saleDate,
                TotalAmount = total,
                DiscountApplied = discount,
                IsPaid = isPaid,
                PaidAt = isPaid ? saleDate : null
            };
            context.SalesInvoices.Add(invoice);

            foreach (var (partIdx, qty) in items)
            {
                context.SalesInvoiceItems.Add(new SalesInvoiceItem
                {
                    ItemId = Guid.NewGuid(),
                    InvoiceId = invoiceId,
                    PartId = partIds[partIdx],
                    Quantity = qty,
                    UnitPrice = (await context.Parts.FindAsync(partIds[partIdx]))?.Price ?? 0
                });
            }
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // PART REQUESTS (8 requests - various statuses)
        // ═══════════════════════════════════════════════════════════════════════

        var partRequestData = new[]
        {
            (0, "LED Headlight Bulbs", "H7 LED bulbs for better visibility", -10, PartRequestStatus.Pending),
            (1, "Cabin Air Filter", "HEPA cabin air filter for Hyundai Creta", -15, PartRequestStatus.Fulfilled),
            (2, "Performance Exhaust", "Aftermarket exhaust system", -20, PartRequestStatus.Rejected),
            (3, "Roof Rack", "Universal roof rack system", -5, PartRequestStatus.Pending),
            (4, "Floor Mats", "All-weather floor mats for Hyundai i20", -8, PartRequestStatus.Fulfilled),
            (5, "Dash Cam", "Front and rear dash camera", -3, PartRequestStatus.Pending),
            (0, "Mud Flaps", "Custom mud flaps for Toyota Corolla", -12, PartRequestStatus.Fulfilled),
            (1, "Spoiler", "Rear spoiler for Maruti Swift", -6, PartRequestStatus.Rejected)
        };

        foreach (var (custIdx, name, desc, daysAgo, status) in partRequestData)
        {
            context.PartRequests.Add(new PartRequest
            {
                RequestId = Guid.NewGuid(),
                CustomerId = customerIds[custIdx],
                PartName = name,
                Description = desc,
                Status = status,
                RequestedAt = DateTime.UtcNow.AddDays(daysAgo)
            });
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // REVIEWS (10 reviews - mix of approved and pending)
        // ═══════════════════════════════════════════════════════════════════════

        var reviewData = new[]
        {
            (0, 5, "Excellent service! Very professional staff and quick turnaround time.", -35, true),
            (1, 4, "Good service overall. Fair pricing and quality work.", -28, true),
            (2, 5, "Best vehicle service center in Kathmandu! Highly recommend.", -22, true),
            (3, 5, "Great experience. Staff explained everything clearly.", -18, true),
            (4, 4, "Professional service. Slightly expensive but worth it.", -12, true),
            (5, 5, "Amazing service! Will definitely come back.", -8, true),
            (0, 5, "Second visit and still impressed. Consistent quality.", -5, true),
            (1, 4, "Good work on brake service. Took a bit longer than expected.", -3, false),
            (2, 5, "Transparent pricing and excellent customer service.", -1, false),
            (3, 5, "Very satisfied with the oil change service.", 0, false)
        };

        foreach (var (custIdx, rating, comment, daysAgo, approved) in reviewData)
        {
            context.Reviews.Add(new Review
            {
                ReviewId = Guid.NewGuid(),
                CustomerId = customerIds[custIdx],
                Rating = rating,
                Comment = comment,
                IsApproved = approved,
                CreatedAt = DateTime.UtcNow.AddDays(daysAgo)
            });
        }

        await context.SaveChangesAsync();

        // ═══════════════════════════════════════════════════════════════════════
        // NOTIFICATIONS (12 notifications - realistic mix)
        // ═══════════════════════════════════════════════════════════════════════

        var notificationData = new[]
        {
            (adminUserId, "Low stock alert: Wiper Blades (Pair) - Only 8 units remaining", NotificationType.LowStock, -2, false),
            (customerUserIds[0], $"Your appointment for Brake Inspection is confirmed for {DateTime.UtcNow.AddDays(3):MMM dd, yyyy}", NotificationType.General, -1, false),
            (customerUserIds[1], "Your part request for 'Cabin Air Filter' has been fulfilled", NotificationType.General, -15, true),
            (adminUserId, "Low stock alert: Spark Plugs (Set of 4) - Only 18 units remaining", NotificationType.LowStock, -5, true),
            (customerUserIds[2], $"Your appointment for Engine Check has been completed", NotificationType.General, -3, true),
            (customerUserIds[3], $"Your appointment for Oil Change is confirmed for {DateTime.UtcNow.AddDays(7):MMM dd, yyyy}", NotificationType.General, -2, false),
            (adminUserId, "Low stock alert: Battery 12V 65Ah - Only 12 units remaining", NotificationType.LowStock, -10, true),
            (customerUserIds[4], "Your part request for 'Floor Mats' has been fulfilled", NotificationType.General, -8, true),
            (customerUserIds[5], $"Your appointment for AC Service has been completed", NotificationType.General, -5, true),
            (customerUserIds[0], "Thank you for your review! It has been approved.", NotificationType.General, -4, true),
            (adminUserId, "Low stock alert: Brake Pads (Rear) - Only 22 units remaining", NotificationType.LowStock, -1, false),
            (customerUserIds[1], $"Your appointment for Tire Rotation is confirmed for {DateTime.UtcNow.AddDays(5):MMM dd, yyyy}", NotificationType.General, 0, false)
        };

        foreach (var (userId, message, type, daysAgo, isRead) in notificationData)
        {
            context.Notifications.Add(new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                Message = message,
                Type = type,
                IsRead = isRead,
                CreatedAt = DateTime.UtcNow.AddDays(daysAgo).AddHours(Random.Shared.Next(0, 24))
            });
        }

        await context.SaveChangesAsync();
    }
}
