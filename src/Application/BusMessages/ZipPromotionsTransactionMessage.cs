using PromotionsEngine.Domain.Enumerations;
using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.BusMessages;

/// <summary>
/// We may need to add more properties to this class as we go along.
/// </summary>
[ExcludeFromCodeCoverage]
public class PromotionsEngineTransactionMessage
{
    /// <summary>
    /// The transaction type of the message that represents the event type that created the message.
    /// <see cref="TransactionTypeEnum"/>
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// The internal order Id.
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// The Stripe authorizationId of the transaction.
    /// </summary>
    public string AuthorizationId { get; set; } = string.Empty;

    /// <summary>
    /// The External Merchant Id associated with the transaction.
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The External Merchant Name associated with the transaction.
    /// </summary>
    public string MerchantName { get; set; } = string.Empty;

    /// <summary>
    /// The reference number the merchant will use to correlate this order amount increase.
    /// </summary>
    public string MerchantReference { get; set; } = string.Empty;

    /// <summary>
    /// The dollar amount of the transaction.
    /// This can represent the change in amount for an order increased event.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// For Order amount increased events, this will represent the new order amount.
    /// Null for all other events.
    /// </summary>
    public decimal? NewOrderAmount { get; set; }

    /// <summary>
    /// For Order amount increased events, this will represent the old order amount.
    /// Null for all other events.
    /// </summary>
    public decimal? OlderOrderAmount { get; set; }

    /// <summary>
    /// The currency code of the transaction.
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    //MerchantInformation??

    /// <summary>
    /// The customer id associated with the transaction.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the specific location this order was created at. May be different than the merchant name.
    /// </summary>
    public string OrderLocationName { get; set; } = string.Empty;

    /// <summary>
    /// Transaction Identifier, if applicable (e.g. Order Refunded).
    /// Use <see cref="AuthorizationId"/> when available.
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Indicates the territory of the customer associated with this order
    /// </summary>
    public string Territory { get; set; } = string.Empty;

    /// <summary>
    /// Indicates a new tax amount for this order, if provided"
    /// </summary>
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// Indicates a new shipping amount for this order, if provided
    /// </summary>
    public decimal? ShippingAmount { get; set; }

    /// <summary>
    /// Customer fee for the order
    /// </summary>
    public decimal? Fee { get; set; }

    /// <summary>
    ///  Gets or sets the balance value on the purchase ledger.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets the total of all captures that the merchant has made for this purchase.
    /// </summary>
    public decimal? TotalGatewayCaptured { get; set; }

    /// <summary>
    /// Gets or sets the total of all refunds that the merchant has made for this purchase.
    /// </summary>
    public decimal? TotalGatewayRefunded { get; set; }

    /// <summary>
    /// Aggregated transaction details per merchant (where available).
    /// </summary>
    public List<MerchantTransactionDetail> TransactionDetails { get; set; } = new List<MerchantTransactionDetail>();

    public class MerchantTransactionDetail
    {
        /// <summary>
        /// Merchant/Location Name.
        /// </summary>
        public string MerchantName { get; set; } = string.Empty;

        /// <summary>
        /// Sum captured amount for the specified merchant
        /// </summary>
        public decimal TotalGatewayCaptured { get; set; }

        /// <summary>
        /// Sum refunded amount for the specified merchant.
        /// </summary>
        public decimal TotalGatewayRefunded { get; set; }
    }
}