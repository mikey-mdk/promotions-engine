using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Constants;

[ExcludeFromCodeCoverage]
public static class CTransactionType
{
    public const string OrderCreated = "OrderCreated";

    public const string OrderAmountIncreased = "OrderAmountIncreased";

    public const string OrderRefunded = "OrderRefunded";

    public const string OrderSettled = "OrderSettled";

    public const string Unknown = "Unknown";

    public static List<string> GetAll()
    {
        return new List<string>
        {
            OrderCreated,
            OrderAmountIncreased,
            OrderRefunded,
            OrderSettled,
        };
    }
}