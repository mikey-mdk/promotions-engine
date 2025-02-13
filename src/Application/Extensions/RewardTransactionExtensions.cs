using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Extensions;

public static class RewardTransactionExtensions
{
    /// <summary>
    /// Calculate the current total order amount.
    /// </summary>
    public static decimal CalculateOrderAmount(this IEnumerable<RewardTransaction> rewardTransactions)
    {
        return rewardTransactions.Sum(rewardTransaction => rewardTransaction.TransactionType switch
        {
            CTransactionType.OrderCreated => rewardTransaction.Amount,
            CTransactionType.OrderAmountIncreased => rewardTransaction.Amount,
            CTransactionType.OrderRefunded => -rewardTransaction.Amount,
            CTransactionType.OrderSettled => 0,
            _ => throw new InvalidOperationException($"Invalid transaction type: {rewardTransaction.TransactionType}")
        });
    }
}
