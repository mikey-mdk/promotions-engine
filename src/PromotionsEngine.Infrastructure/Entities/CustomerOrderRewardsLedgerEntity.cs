using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Infrastructure.Entities;

[ExcludeFromCodeCoverage]
public class CustomerOrderRewardsLedgerEntity : EntityBase
{
    /// <summary>
    /// The orderId that this rewards ledger is associated with.
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// This customerId that this rewards ledger is associated with.
    /// This is also the cosmos container partition key.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The MerchantEntity.
    /// </summary>
    public MerchantEntity Merchant { get; set; } = new();

    /// <summary>
    /// The PromotionsEntity.
    /// </summary>
    public PromotionEntity Promotion { get; set; } = new();

    /// <summary>
    /// The list of reward transactions.
    /// </summary>
    public List<RewardTransactionEntity> RewardTransactions { get; set; } = new();

    /// <summary>
    /// The current sum of all the balances of the RewardTransactions.
    /// </summary>
    public decimal RewardBalance { get; set; }
}