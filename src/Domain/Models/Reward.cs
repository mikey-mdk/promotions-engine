namespace PromotionsEngine.Domain.Models;

public class Reward
{
    public string OrderId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string PromotionId { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}