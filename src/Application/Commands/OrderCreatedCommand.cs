using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Application.Commands;

[ExcludeFromCodeCoverage]
public class OrderCreatedCommand : CommandBase
{
    public override string TransactionType { get; } = CTransactionType.OrderCreated;

    public string OrderId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string ExternalMerchantId { get; set; } = string.Empty;

    public string MerchantName { get; set; } = string.Empty;

    public string AuthorizationId { get; set; } = string.Empty;

    public decimal OrderAmount { get; set; }
}