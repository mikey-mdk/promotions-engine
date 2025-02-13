using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Domain.Repositories.Interfaces;

public interface IPromotionsRepository
{
    /// <summary>
    /// This method is used to find a promotions document by the provided Id.
    /// </summary>
    /// <param name="promotionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Promotion> GetPromotionByIdAsync(string promotionId, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to find a list of promotion documents for the provided merchantId.
    /// </summary>
    /// <param name="merchantId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Promotion>> GetPromotionsByMerchantIdAsync(string merchantId, CancellationToken cancellationToken);

    /// <summary>
    /// This method will perform a queryRequest on the Promotions container based on the provided queryRequest object.
    /// </summary>
    /// <param name="queryRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Promotion>> GetPromotionsFromQueryAsync(GetPromotionsQueryRequest queryRequest, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to create the Promotions document in the container.
    /// </summary>
    Task<Promotion> CreatePromotionAsync(Promotion promotion, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to Update the Promotions document in the container.
    /// </summary>
    Task<Promotion> UpdatePromotionAsync(Promotion promotion, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to mark the Promotions document as deleted in the container.
    /// </summary>
    Task<Promotion> DeletePromotionAsync(string promotionId, CancellationToken cancellationToken);
}