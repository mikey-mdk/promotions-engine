using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Application.Commands;

[ExcludeFromCodeCoverage]
public class OrderRefundedCommand : CommandBase
{
    public override string TransactionType { get; } = CTransactionType.OrderRefunded;

    /// <summary>
    /// Transaction Identifier
    /// </summary>
    public string TransactionId { get; init; } = string.Empty;

    /// <summary>
    /// Order Id.
    /// </summary>
    public string OrderId { get; init; } = string.Empty;

    /// <summary>
    /// Custome Id.
    /// </summary>
    public string CustomerId { get; init; } = string.Empty;

    /// <summary>
    /// Order Merchant Id.
    /// </summary>
    public string ExternalMerchantId { get; init; } = string.Empty;

    /// <summary>
    /// Refunded amount.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Currency code of the refund.
    /// </summary>
    public string CurrencyCode { get; init; } = string.Empty;
}
