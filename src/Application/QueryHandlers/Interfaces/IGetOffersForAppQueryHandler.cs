using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Application.Queries;

namespace PromotionsEngine.Application.QueryHandlers.Interfaces;

public interface IGetOffersForAppQueryHandler
{
    /// <summary>
    /// This method will return all active promotions across all merchants.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GetAppOffersDto> GetOffersForAppAsync(GetOffersForAppQuery query, CancellationToken cancellationToken);
}