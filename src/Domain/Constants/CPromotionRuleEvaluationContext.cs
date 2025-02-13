using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Constants;

[ExcludeFromCodeCoverage]
public static class CPromotionRuleEvaluationContext
{
    //Represents the rules evaluation occurring when an OrderCreated event is received.
    public const string OrderCreated = "OrderCreated";

    //Represents the rules evaluation occurring when an OrderRefunded event is received.
    public const string OrderRefunded = "OrderRefunded";

    //Represent the rules evaluation occurring for Checkout offers presentation workflow.
    public const string CheckoutPresentation = "CheckoutPresentation";

    //Represent the rules evaluation occurring for App offers presentation workflow.
    public const string AppPresentation = "AppPresentation";

    //All contexts
    public const string All = "All";

    public static List<string> GetAll()
    {
        return new List<string>
        {
            OrderCreated,
            OrderRefunded,
            CheckoutPresentation,
            AppPresentation,
            All
        };
    }
}