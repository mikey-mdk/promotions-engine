using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Constants;

[ExcludeFromCodeCoverage]
public static class CRewardRateType
{
    public const string Fixed = "Fixed";

    public const string Percentage = "Percentage";

    public static List<string> GetAll()
    {
        return new List<string>
        {
            Fixed,
            Percentage
        };
    }
}