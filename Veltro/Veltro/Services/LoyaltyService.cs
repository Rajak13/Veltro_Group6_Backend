using Veltro.Services.Interfaces;

namespace Veltro.Services;

/// <summary>Implements the loyalty discount program: 10% off purchases over 5000.</summary>
public class LoyaltyService : ILoyaltyService
{
    private const decimal DiscountThreshold = 5000m;
    private const decimal DiscountRate = 0.10m;

    /// <summary>Returns 10% of subtotal if subtotal exceeds 5000, otherwise 0.</summary>
    public decimal CalculateDiscount(decimal subtotal) =>
        subtotal > DiscountThreshold ? Math.Round(subtotal * DiscountRate, 2) : 0m;
}
