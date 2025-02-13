using PromotionsEngine.Application.Dtos.Promotion;
using PromotionsEngine.Application.Requests.Promotions;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.Services.Interfaces;

public interface IPromotionsService
{
    /// <summary>
    /// This method is used to find the Promotion by the provided id.
    /// </summary>
    Task<PromotionDto> GetPromotionByIdAsync(string promotionId, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to create the Promotion document.
    /// </summary>
    Task<PromotionDto> CreatePromotionAsync(CreatePromotionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method will perform a query on the Promotions container based on the provided request object.
    /// This method is intended to be used sparsely and therefor is included in the PromotionService.
    /// If this method is used frequently for more consumer facing operations we should separate it out into its own Query workflow for scalability reasons.  
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<PromotionDto>> GetPromotionsFromQueryAsync(GetPromotionsQueryRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to update the Promotion document.
    /// </summary>
    Task<PromotionDto> UpdatePromotionAsync(UpdatePromotionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to mark the Promotion document as deleted.
    /// </summary>
    Task<PromotionDto> DeletePromotionAsync(string promotionId, CancellationToken cancellationToken);
}
