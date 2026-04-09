namespace Veltro.Services.Interfaces;

/// <summary>Loyalty program discount calculation contract.</summary>
public interface ILoyaltyService
{
    /// <summary>Returns the discount amount to apply. 10% if subtotal > 5000, else 0.</summary>
    decimal CalculateDiscount(decimal subtotal);
}
