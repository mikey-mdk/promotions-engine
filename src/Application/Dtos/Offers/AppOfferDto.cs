using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Application.Dtos.Offers;

[ExcludeFromCodeCoverage]
public class AppOfferDto
{
    /// <summary>
    /// The name of the promotion.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The reward type of the promotion. <see cref="CPromotionType"/>
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The start date of the promotion.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The percentage of the order used to calculate the reward. Mutually Exclusive to RateFixed.
    /// </summary>
    public decimal RatePercentage { get; set; }

    /// <summary>
    /// The fixed amount to be rewarded. Mutually Exclusive to RatePercentage.
    /// </summary>
    public decimal RateFixed { get; set; }

    /// <summary>
    /// The merchant Id used in external systems such as Contentful.
    /// </summary>
    public string ExternalMerchantId { get; set; } = string.Empty;
}