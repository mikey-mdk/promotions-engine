using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Domain.Models;

[ExcludeFromCodeCoverage]
public class RewardTransaction
{
    public string TransactionType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string AuthorizationId { get; set; } = string.Empty;

    public string OrderId { get; set; } = string.Empty;

    public string MerchantName { get; set; } = string.Empty;

    public string MerchantId { get; set; } = string.Empty;

    public string TransactionId { get; set; } = string.Empty;


    public static RewardTransaction Settled(string orderId, decimal amount) => new ()
    {
        TransactionType = CTransactionType.OrderSettled,
        OrderId = orderId,
        Amount = amount
    };
}