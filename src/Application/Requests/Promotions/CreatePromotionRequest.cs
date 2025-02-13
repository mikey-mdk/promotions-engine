using PromotionsEngine.Application.Dtos.PromotionRules;

namespace PromotionsEngine.Application.Requests.Promotions;

public class CreatePromotionRequest
{
    /// <summary>
    /// The merchant this promotion is associated with.
    /// </summary>
    public string MerchantId { get; init; } = string.Empty;

    /// <summary>
    /// The name given to this promotion.
    /// </summary>
    public string PromotionName { get; init; } = string.Empty;

    /// <summary>
    /// Promotion Rules
    /// </summary>
    public PromotionRulesDto PromotionRules { get; init; } = new();

    /// <summary>
    /// The type of Promotion.
    /// </summary>
    public int? PromotionTypeEnum { get; init; }

    /// <summary>
    /// The description of the Promotion.
    /// </summary>
    public string PromotionDescription { get; init; } = string.Empty;

    /// <summary>
    /// The date the promotion starts.
    /// </summary>
    public DateTime PromotionStartDate { get; init; }

    /// <summary>
    /// The date the promotion ends.
    /// </summary>
    public DateTime PromotionEndDate { get; init; }

    /// <summary>
    /// The rate type for the promotion reward distribution.
    /// </summary>
    public int? RewardRateTypeEnum { get; init; }

    /// <summary>
    /// The amount of the promotion reward distribution.
    /// </summary>
    public decimal RateAmount { get; init; }
}