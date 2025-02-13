using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Application.Queries;

namespace PromotionsEngine.Application.QueryHandlers.Interfaces;

public interface IGetOffersForCheckoutQueryHandler
{
    /// <summary>
    /// This method will return the valid offers for the provided checkout context.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<GetCheckoutOffersDto> GetOffersForCheckoutAsync(GetOffersForCheckoutQuery query, CancellationToken cancellationToken);
}