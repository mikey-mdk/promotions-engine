namespace PromotionsEngine.Domain.Models;

/// <summary>
/// This class will be used to define the rules for a promotion
/// </summary>
public class PromotionRules
{
    /// <summary>
    /// This is the number of times a promotion can be redeemed.
    /// Otherwise known as the number of individual transactions that can be made for a promotion.
    /// </summary>
    public int? NumberOfTimesRedeemable { get; set; }

    /// <summary>
    /// This is the minimum dollar amount that an order must be for in order to qualify for this promotion.
    /// </summary>
    public decimal MinimumTransactionAmount { get; set; }

    /// <summary>
    /// This is a maximum dollar amount of a single order that can still qualify for a promotion.
    /// </summary>
    public decimal? MaximumTransactionAmount { get; set; }

    /// <summary>
    /// This is the total dollar amount of rewards that can be issued for a promotion.
    /// </summary>
    public decimal? TotalRewardsAmount { get; set; }

    /// <summary>
    /// This is the number of time a promotion can be redeemed per customer. This wont apply to checkout and will likely always be 1.
    /// Putting it here as a rule in case there is a use case we haven't considered.
    /// </summary>
    public int NumberOfRedemptionsPerCustomer { get; set; } = 1;

    /// <summary>
    /// This is the total number of customers that can redeem a promotion.
    /// </summary>
    public int? TotalNumberOfCustomers { get; set; }
}
