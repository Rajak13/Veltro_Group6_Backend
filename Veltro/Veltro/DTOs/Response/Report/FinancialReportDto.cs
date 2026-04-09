namespace Veltro.DTOs.Response.Report;

/// <summary>Financial summary report for a given period.</summary>
public class FinancialReportDto
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal NetProfit => TotalSales - TotalPurchases;
    public List<TopSellingPartDto> TopSellingParts { get; set; } = new();
}

/// <summary>Part with highest sales volume in the report period.</summary>
public class TopSellingPartDto
{
    public Guid PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
