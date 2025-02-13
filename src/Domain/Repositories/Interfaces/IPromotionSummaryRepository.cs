using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Repositories.Interfaces;

/// <summary>
/// The Promotion Summary will be written to and read from frequently which negates the benefit of caching and necessitates its own repository.
/// The PromotionSummary will not be included in the Promotion domain model to make it explicit that it needs to be operated on separately.
/// </summary>
public interface IPromotionSummaryRepository
{
    /// <summary>
    /// This method will be used to get the Promotion Summary for a given Promotion.
    /// </summary>
    /// <param name="promotionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PromotionSummary?> GetPromotionSummaryAsync(string promotionId, CancellationToken cancellationToken);
    
    /// <summary>
    /// This method will be used to update the Promotion Summary for a given Promotion whenever redemption or modification to aggregate promotion usage occurs.
    /// </summary>
    /// <param name="promotionSummary"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PromotionSummary?> UpdatePromotionSummaryAsync(PromotionSummary promotionSummary, CancellationToken cancellationToken);
}