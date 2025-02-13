using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Constants;

/// <summary>
/// Bringing this over from Beam but its unclear if this will be used.
/// </summary>
[ExcludeFromCodeCoverage]
public static class CPromotionType
{
    public const string CashBack = "CashBack";

    public const string Discount = "Discount";

    //etc...

    public static List<string> GetAll()
    {
        return new List<string>
        {
            CashBack,
            Discount
        };
    }
}