using Veltro.DTOs.Response.Report;

namespace Veltro.Services.Interfaces;

/// <summary>Reporting service contract.</summary>
public interface IReportService
{
    Task<FinancialReportDto> GetFinancialReportAsync(string period);
    Task<IEnumerable<CustomerReportDto>> GetTopSpendersAsync(int top = 10);
    Task<IEnumerable<CustomerReportDto>> GetRegularCustomersAsync(int minPurchases = 3);
    Task<IEnumerable<CustomerReportDto>> GetOverdueCreditsAsync();
}
