using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Constants;

[ExcludeFromCodeCoverage]
public static class CMerchantTypes
{
    public const string Retailer = "Retailer";

    public const string Brand = "Brand";

    //etc...

    public static List<string> GetAll()
    {
        return new List<string>
        {
            Retailer,
            Brand
        };
    }
}