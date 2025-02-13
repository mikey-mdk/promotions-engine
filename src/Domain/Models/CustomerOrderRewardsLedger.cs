using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Models;

[ExcludeFromCodeCoverage]
public class CustomerOrderRewardsLedger
{
    /// <summary>
    /// The Merchant this Promotion reward ledger is associated with.
    /// </summary>
    public Merchant Merchant { get; set; } = new();

    /// <summary>
    /// The Promotion this reward ledger is associated with.
    /// </summary>
    public Promotion Promotion { get; set; } = new();

    /// <summary>
    /// The customer this reward ledger is associated with.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The internal order Id.
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// The running list of all the transactions received related to rewards allocation.
    /// </summary>
    public List<RewardTransaction> RewardTransactions { get; set; } = new List<RewardTransaction>();

    /// <summary>
    /// The current sum of all the balances of the RewardTransactions
    /// </summary>
    public decimal RewardBalance { get; set; }
}