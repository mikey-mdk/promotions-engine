#nullable disable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.BusMessages.PurchaseLedger;

[ExcludeFromCodeCoverage]
[Description("Raised when the order created by the purchase ledger is refunded")]
public class PurchaseLedgerOrderRefunded
{
    [Description("The id of the purchase ledger")]
    public Guid PurchaseId { get; set; }

    [Description("The id of the order created by the purchase ledger (will match purchase ledger id)")]
    public Guid OrderId { get; set; }

    [Description("The identifier of the customer whose order is being refunded")]
    public Guid? CustomerId { get; set; }

    [Description("The identifier of the merchant initiating the refund")]
    public Guid? MerchantId { get; set; }

    [Description("The id of the Stripe transaction")]
    public string TransactionId { get; set; }

    [Description("Amount the order was refunded")]
    public decimal Amount { get; set; }

    [Description("The currency code")]
    public string Currency { get; set; }

    [Description("Date the order was refunded")]
    public DateTimeOffset CreatedDate { get; set; }

    [Description("Id of the admin when request is manually triggered")]
    public Guid? RequesterCustomerId { get; set; }

    [Description("Email of the admin when request is manually triggered")]
    public string RequesterEmail { get; set; }

    [Description("The type of the product")]
    public string ProductType { get; set; }
}
