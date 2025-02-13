using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class MerchantTypeEnum : EnumerationBase
{
    public static readonly MerchantTypeEnum Retailer = new(1, nameof(Retailer));

    public static readonly MerchantTypeEnum Brand = new(2, nameof(Brand));

    public MerchantTypeEnum(int id, string name) : base(id, name)
    {
    }
}