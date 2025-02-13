using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class PromotionRuleTypeEnum : EnumerationBase
{
    public static readonly PromotionRuleTypeEnum NumberOfTimesRedeemableRule = new(1, nameof(NumberOfTimesRedeemableRule));

    public static readonly PromotionRuleTypeEnum TransactionAmountRule = new(2, nameof(TransactionAmountRule));

    public PromotionRuleTypeEnum(int id, string name) : base(id, name)
    {
    }
}