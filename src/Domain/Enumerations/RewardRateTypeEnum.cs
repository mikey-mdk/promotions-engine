using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class RewardRateTypeEnum : EnumerationBase
{
    public static readonly RewardRateTypeEnum Fixed = new(1, nameof(Fixed));

    public static readonly RewardRateTypeEnum Percentage = new(2, nameof(Percentage));

    public RewardRateTypeEnum(int id, string name) : base(id, name)
    {
    }
}