namespace Veltro.Models;

/// <summary>User roles for role-based access control.</summary>
public enum UserRole { Admin, Staff, Customer }

/// <summary>Appointment lifecycle states.</summary>
public enum AppointmentStatus { Pending, Confirmed, Completed, Cancelled }

/// <summary>Part request fulfilment states.</summary>
public enum PartRequestStatus { Pending, Fulfilled, Rejected }

/// <summary>Notification category types.</summary>
public enum NotificationType { LowStock, CreditOverdue, General }
