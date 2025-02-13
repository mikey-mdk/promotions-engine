using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.Dtos.Offers;

/// <summary>
/// The CheckoutOffer is considered a valid instance of a Promotion with the calculated discount amount.
/// </summary>
[ExcludeFromCodeCoverage]
public class CheckoutOfferDto
{
    /// <summary>
    /// Merchant Id.
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The external merchant id provided during merchant setup.
    /// </summary>
    public string ExternalMerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the merchant.
    /// </summary>
    public string MerchantName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the promotion.
    /// </summary>
    public string PromotionName { get; set; } = string.Empty;

    /// <summary>
    /// The text describing the promotion.
    /// </summary>
    public string PromotionDescription { get; set; } = string.Empty;

    /// <summary>
    /// The start date of the promotion.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the promotion.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The original amount of the order.
    /// </summary>
    public decimal OrderAmount { get; set; }

    /// <summary>
    /// The discount amount calculated for the order.
    /// </summary>
    public decimal? DiscountAmount { get; set; }
}