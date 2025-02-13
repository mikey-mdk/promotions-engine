using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Models;

[ExcludeFromCodeCoverage]
public class PromotionSummary
{
    /// <summary>
    /// This will be the same id used for uniquely identifying a promotion
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The total number of unique customers that have redeem rewards for this promotion.
    /// </summary>
    public int TotalNumberOfCustomers { get; set; }

    /// <summary>
    /// The total number of times this promotion has be redeemed, regardless of unique customers.
    /// </summary>
    public int NumberOfTimesRedeemed { get; set; }

    /// <summary>
    /// Total dollar amount redeemed for this promotion.
    /// </summary>
    public decimal TotalAmountRedeemed { get; set; }
}