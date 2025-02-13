using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Constants;

[ExcludeFromCodeCoverage]
public static class CBusinessTypes
{
    public const string Electronics = "Electronics";

    public const string Fashion = "Fashion";

    //etc...

    public static List<string> GetAll()
    {
        return new List<string>
        {
            Electronics,
            Fashion
        };
    }
}