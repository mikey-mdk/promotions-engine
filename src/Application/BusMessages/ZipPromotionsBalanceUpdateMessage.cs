using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Application.BusMessages;

[ExcludeFromCodeCoverage]
public class PromotionsEngineBalanceUpdateMessage
{
    /// <summary>
    /// The calculated reward amount for a given event. This can be a positive, negative, or zero value.
    /// For refunds and settled events this will be the difference between the original reward amount and the refund amount.
    /// For settled events where the amount hasn't changed this will be zero.
    /// </summary>
    public decimal RewardAmount { get; }

    /// <summary>
    /// The order id that the reward amount is associated with.
    /// </summary>
    public string OrderId { get; }

    /// <summary>
    /// The customer id that the reward is associated with.
    /// </summary>
    public string CustomerId { get; }

    /// <summary>
    /// <see cref="CTransactionType"/>>
    /// </summary>
    public string TransactionType { get; }

    private PromotionsEngineBalanceUpdateMessage(decimal rewardAmount, string orderId, string customerId, string transactionType)
    {
        RewardAmount = rewardAmount;
        OrderId = orderId;
        CustomerId = customerId;
        TransactionType = transactionType;
    }

    public static PromotionsEngineBalanceUpdateMessage Created(decimal rewardAmount, string orderId, string customerId)
        => new(rewardAmount, orderId, customerId, CTransactionType.OrderCreated);

    public static PromotionsEngineBalanceUpdateMessage Refunded(decimal rewardDifference, string orderId, string customerId)
        => new(rewardDifference, orderId, customerId, CTransactionType.OrderRefunded);

    public static PromotionsEngineBalanceUpdateMessage Settled(decimal rewardDifference, string orderId, string customerId)
        => new (rewardDifference, orderId, customerId, CTransactionType.OrderSettled);
}