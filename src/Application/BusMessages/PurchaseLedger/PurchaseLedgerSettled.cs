#nullable disable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.BusMessages.PurchaseLedger;

/* Copied from the Wallet service */
[ExcludeFromCodeCoverage]
[Description("Raised when the purchase ledger is settled")]
public class PurchaseLedgerSettled
{
    [Description("The id of the purchase ledger")]
    public Guid PurchaseId { get; set; }

    [Description("Gets or sets the balance value on the purchase ledger.")]
    public decimal Balance { get; set; }

    [Description("Gets or sets the total of all captures that the merchant has made for this purchase.")]
    public decimal? TotalGatewayCaptured { get; set; }

    [Description("Gets or sets the total of all refunds that the merchant has made for this purchase.")]
    public decimal? TotalGatewayRefunded { get; set; }

    [Description("The currency code")]
    public string Currency { get; set; }

    [Description("Date the purchase ledger was settled")]
    public DateTimeOffset CreatedDate { get; set; }

    [Description("Gets or sets the CustomerId of the requester when this event has been manually triggered.")]
    public Guid? RequesterCustomerId { get; set; }

    [Description("Gets or sets the email of the requester when this event has been manually triggered.")]
    public string RequesterEmail { get; set; }

    [Description("Tracing information to determine the origin of the event")]
    public string Origin { get; set; }

    [Description("Aggregated transaction details per merchant (where available).")]
    public List<MerchantTransactionDetail> TransactionDetails { get; set; }

    public class MerchantTransactionDetail
    {
        [Description("Merchant/Location Name.")]
        public string MerchantName { get; set; }

        [Description("Sum captured amount for the specified merchant.")]
        public decimal TotalGatewayCaptured { get; set; }

        [Description("Sum refunded amount for the specified merchant.")]
        public decimal TotalGatewayRefunded { get; set; }
    }
}
