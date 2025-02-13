using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Infrastructure.Entities;

[ExcludeFromCodeCoverage]
public class PromotionEntity : EntityBase
{
    /// <summary>
    /// The merchant this promotion is associated with.
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The name given to this promotion.
    /// </summary>
    public string PromotionName { get; set; } = string.Empty;

    public PromotionRulesEntity PromotionRules { get; set; } = new();

    /// <summary>
    /// The type of Promotion.
    /// </summary>
    public int? PromotionTypeEnum { get; set; }

    /// <summary>
    /// The description of the Promotion.
    /// </summary>
    public string PromotionDescription { get; set; } = string.Empty;

    /// <summary>
    /// The date the promotion starts.
    /// </summary>
    public DateTime PromotionStartDate { get; set; }

    /// <summary>
    /// The date the promotion ends.
    /// </summary>
    public DateTime PromotionEndDate { get; set; }

    /// <summary>
    /// The rate type for the promotion reward distribution.
    /// </summary>
    public int? RewardRateTypeEnum { get; set; }

    /// <summary>
    /// The amount of the promotion reward distribution.
    /// </summary>
    public decimal RateAmount { get; set; }

    /// <summary>
    /// Indicates if the Promotion is currently active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Determines if the promotion has been deleted.
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// The datetime that the promotion was created.
    /// </summary>
    public DateTime CreatedDateTime { get; set; }

    /// <summary>
    /// The datetime of the most recent  modification to the promotion.
    /// </summary>
    public DateTime? ModifiedDateTime { get; set; }

    /// <summary>
    /// The unique list of customer ids that have redeemed this promotion.
    /// </summary>
    public List<string> CustomerIds { get; set; } = new();

    /// <summary>
    /// The current schema version of the Promotion document in CosmosDB that the App is supporting.
    /// </summary>
    public int SchemaVersion { get; set; }
}
