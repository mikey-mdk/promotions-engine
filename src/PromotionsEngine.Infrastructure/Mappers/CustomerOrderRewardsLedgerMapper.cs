using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.Mappers;

public static class CustomerOrderRewardsLedgerMapper
{
    public static CustomerOrderRewardsLedger MapToDomain(this CustomerOrderRewardsLedgerEntity entity)
    {
        return new CustomerOrderRewardsLedger
        {
            OrderId = entity.OrderId,
            CustomerId = entity.CustomerId,
            Merchant = entity.Merchant.MapToDomain(),
            Promotion = entity.Promotion.MapToDomain(),
            RewardTransactions = RewardTransactionEntityMapper.MapToRewardTransactionDomain(entity.RewardTransactions),
            RewardBalance = entity.RewardBalance
        };
    }

    public static CustomerOrderRewardsLedgerEntity MapToEntity(this CustomerOrderRewardsLedger customerOrderRewardsLedger)
    {
        return new CustomerOrderRewardsLedgerEntity
        {
            OrderId = customerOrderRewardsLedger.OrderId,
            CustomerId = customerOrderRewardsLedger.CustomerId,
            Merchant = customerOrderRewardsLedger.Merchant.MapToEntity(),
            Promotion = customerOrderRewardsLedger.Promotion.MapToEntity(),
            RewardTransactions = RewardTransactionEntityMapper.MapToRewardTransactionEntity(customerOrderRewardsLedger.RewardTransactions),
            RewardBalance = customerOrderRewardsLedger.RewardBalance
        };
    }
}