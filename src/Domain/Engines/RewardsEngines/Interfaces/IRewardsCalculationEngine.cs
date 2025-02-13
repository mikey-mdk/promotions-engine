using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;

public interface IRewardsCalculationEngine
{
    /// <summary>
    /// Calculate the reward amount for the given request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<decimal> CalculateRewardsAsync(RewardsCalculationRequest request);

    /// <summary>
    /// Find the largest reward for the provided promotions.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<Reward> FindLargestRewardForOrderAsync(FindLargestRewardForOrderRequest request);
}