using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class PromotionRuleEvaluationTypeEnum : EnumerationBase
{
    public static readonly PromotionRuleEvaluationTypeEnum Rewards = new(1, nameof(Rewards));

    public static readonly PromotionRuleEvaluationTypeEnum Presentation = new(2, nameof(Presentation));

    public static readonly PromotionRuleEvaluationTypeEnum All = new(3, nameof(All));

    public PromotionRuleEvaluationTypeEnum(int id, string name) : base(id, name)
    {
    }
}