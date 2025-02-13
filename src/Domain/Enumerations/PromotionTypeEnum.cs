using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class PromotionTypeEnum : EnumerationBase
{
    public static readonly PromotionTypeEnum Cashback = new(1, nameof(Cashback));

    public static readonly PromotionTypeEnum Discount = new(2, nameof(Discount));

    public PromotionTypeEnum(int id, string name) : base(id, name)
    {
    }
}