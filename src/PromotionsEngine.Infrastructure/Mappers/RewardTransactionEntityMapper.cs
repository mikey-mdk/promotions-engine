using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.Mappers;

public static class RewardTransactionEntityMapper
{
    public static List<RewardTransaction> MapToRewardTransactionDomain(List<RewardTransactionEntity> entities)
    {
        return entities.Select(x => new RewardTransaction
        {
            OrderId = x.OrderId,
            AuthorizationId = x.AuthorizationId,
            MerchantId = x.MerchantId,
            MerchantName = x.MerchantName,
            TransactionType = x.TransactionType,
            TransactionId = x.TransactionId,
        }).ToList();
    }

    public static List<RewardTransactionEntity> MapToRewardTransactionEntity(List<RewardTransaction> rewardTransactions)
    {
        return rewardTransactions.Select(x => new RewardTransactionEntity
        {
            OrderId = x.OrderId,
            AuthorizationId = x.AuthorizationId,
            MerchantId = x.MerchantId,
            MerchantName = x.MerchantName,
            TransactionType = x.TransactionType,
            TransactionId = x.TransactionId,
        }).ToList();
    }
}