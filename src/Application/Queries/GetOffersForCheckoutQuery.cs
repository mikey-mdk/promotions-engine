using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.Queries;

[ExcludeFromCodeCoverage]
public class GetOffersForCheckoutQuery
{
    /// <summary>
    /// Merchant Id.
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The order amount passed by the merchant.
    /// </summary>
    public decimal OrderAmount { get; set; }
}