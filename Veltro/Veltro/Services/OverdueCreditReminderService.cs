using Microsoft.EntityFrameworkCore;
using Veltro.Data;
using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>
/// Background service that runs daily at midnight.
/// Sends overdue credit reminder emails to customers with unpaid invoices older than 1 month.
/// </summary>
public class OverdueCreditReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueCreditReminderService> _logger;

    public OverdueCreditReminderService(IServiceScopeFactory scopeFactory,
        ILogger<OverdueCreditReminderService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Wait until next midnight UTC
            var now = DateTime.UtcNow;
            var nextRun = now.Date.AddDays(1);
            var delay = nextRun - now;
            await Task.Delay(delay, stoppingToken);

            await SendOverdueRemindersAsync();
        }
    }

    /// <summary>Queries overdue customers and sends each a reminder email.</summary>
    private async Task SendOverdueRemindersAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var cutoff = DateTime.UtcNow.AddMonths(-1);

            var overdueCustomers = await context.Customers
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.CreditBalance > 0 &&
                            c.SalesInvoices.Any(si => !si.IsPaid && si.SaleDate < cutoff))
                .ToListAsync();

            foreach (var customer in overdueCustomers)
            {
                var html = $"""
                    <h2>Veltro — Overdue Credit Reminder</h2>
                    <p>Dear {customer.User.FullName},</p>
                    <p>You have an outstanding credit balance of <strong>{customer.CreditBalance:C}</strong>
                    with an unpaid invoice that is over 1 month old.</p>
                    <p>Please settle your balance at your earliest convenience.</p>
                    <br/><p>Veltro Vehicle Parts</p>
                    """;

                await emailService.SendEmailAsync(
                    customer.User.Email,
                    customer.User.FullName,
                    "Veltro — Overdue Credit Balance Reminder",
                    html
                );

                await notificationService.CreateNotificationAsync(
                    customer.UserId,
                    $"Reminder: Your credit balance of {customer.CreditBalance:C} is overdue.",
                    Models.NotificationType.CreditOverdue
                );

                _logger.LogInformation("Overdue reminder sent to {Email}", customer.User.Email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Overdue credit reminder job failed");
        }
    }
}
