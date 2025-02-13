using PromotionsEngine.Domain.Enumerations;

namespace PromotionsEngine.Infrastructure.Entities;

public class RewardTransactionEntity
{
    public string TransactionType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string AuthorizationId { get; set; } = string.Empty;

    public string OrderId { get; set; } = string.Empty;

    public string MerchantName { get; set; } = string.Empty;

    public string MerchantId { get; set; } = string.Empty;

    public string TransactionId { get; set; } = string.Empty;
}