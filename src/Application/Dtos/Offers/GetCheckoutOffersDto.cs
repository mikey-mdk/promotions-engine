using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.Dtos.Offers;

[ExcludeFromCodeCoverage]
public class GetCheckoutOffersDto
{
    /// <summary>
    /// This is a list of valid offers with the discount calculated.
    /// </summary>
    public List<CheckoutOfferDto> CheckoutOffers { get; set; } = new();
}