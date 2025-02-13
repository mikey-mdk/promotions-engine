using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Engines.RewardsEngines.Implementations;

public class RewardsCalculationEngine : IRewardsCalculationEngine
{
    public async Task<decimal> CalculateRewardsAsync(RewardsCalculationRequest request)
    {
        var rewardAmount = 0m;
        if (request.RewardRateType.Equals(RewardRateTypeEnum.Fixed))
        {
            rewardAmount = request.RateAmount;
        }

        if (request.RewardRateType.Equals(RewardRateTypeEnum.Percentage))
        {
            rewardAmount = Math.Round(decimal.Multiply(request.OrderAmount, request.RateAmount) / 100, 2,
                MidpointRounding.AwayFromZero);
        }

        return rewardAmount;
    }

    public async Task<Reward> FindLargestRewardForOrderAsync(FindLargestRewardForOrderRequest request)
    {
        var rewards = new List<Reward>();
        foreach (var promotion in request.Promotions)
        {
            var rewardAmount = await CalculateRewardsAsync(new RewardsCalculationRequest
            {
                OrderAmount = request.Amount,
                RewardRateType = promotion.RewardRateTypeEnum!,
                RateAmount = promotion.RateAmount
            });

            rewards.Add(new Reward
            {
                Amount = rewardAmount,
                CustomerId = request.CustomerId,
                OrderId = request.OrderId,
                PromotionId = promotion.Id
            });
        }

        return rewards.Aggregate((max, next) => next.Amount > max.Amount ? next : max);
    }
}
