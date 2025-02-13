#nullable disable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.BusMessages.PurchaseLedger;

[ExcludeFromCodeCoverage]
[Description("Raised when order created in purchase ledger")]
public class PurchaseLedgerOrderCreated
{
    /// <summary>
    /// Date the order was created
    /// </summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>
    /// The origin that triggers the order being created
    /// </summary>
    public string Origin { get; set; }

    /// <summary>
    /// The type of purchase
    /// </summary>
    public string PurchaseType { get; set; }

    /// <summary>
    /// The type of product
    /// </summary>
    public string ProductType { get; set; }

    /// <summary>
    /// The merchant name of the authorization or transaction request webhook
    /// </summary>
    public string AuthorizationMerchantName { get; set; }

    /// <summary>
    /// The id of the purchase ledger
    /// </summary>
    public Guid PurchaseId { get; set; }

    /// <summary>
    /// The store name from purchase request
    /// </summary>
    public string StoreName { get; set; }

    /// <summary>
    /// Gets or sets the ID of the order
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the merchant the order was placed through
    /// </summary>
    public Guid MerchantId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the payment source that will be used for installment payments
    /// </summary>
    public string PaymentSourceId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the initial authorization that will be captured upon order creation
    /// </summary>
    public string AuthorizationId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the customer that created the purchase
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the amount of the order.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the amount authorized to be captured.  Typically the first installment amount.
    /// </summary>
    public decimal AuthorizedAmount { get; set; }

    /// <summary>
    /// Gets or sets the associated currency of the order.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets the source of the service used for creating this purchase.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the datetime of when the purchase was originally authorized.
    /// </summary>
    public DateTimeOffset AuthorizedDate { get; set; }

    /// <summary>
    /// Gets or sets the reference of the order for the merchant.
    /// </summary>
    public string MerchantReference { get; set; }

    /// <summary>
    /// Gets or sets the order options
    /// </summary>
    public Dictionary<string, string> OrderOptions { get; set; }

    /// <summary>
    /// Gets or sets whether this order has previous order
    /// ex: pre-order flow
    /// </summary>
    public Guid? ParentPurchaseId { get; set; }

    /// <summary>
    /// Authorization codes
    /// </summary>
    public string[] AuthorizationCodes { get; set; }

    /// <summary>
    /// Network Transaction Id
    /// </summary>
    public string NetworkTransactionId { get; set; }

    /// <summary>
    /// Issuing Card Type
    /// </summary>
    public string IssuingCardType { get; set; }

    /// <summary>
    /// Gets or sets the location where the purchase was captured
    /// </summary>
    public Location OrderLocation { get; set; }

    public CustomerOrder CustomerOrderInfo { get; set; }

    public class Location
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Categories { get; set; }

        public string AuthorizationMethod { get; set; }

        public string Phone { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }

    public class CustomerOrder
    {
        /// <summary>
        /// The customer's state derived from billing address
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The customer's country derived from billing address
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The customer's territory derived from phone number
        /// </summary>
        public string Territory { get; set; }

        /// <summary>
        /// The customer's phone number from the checkout
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The customer's first name from the checkout
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The customer's last name from the checkout
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The customer's email from the checkout
        /// </summary>
        public string Email { get; set; }
    }
}
