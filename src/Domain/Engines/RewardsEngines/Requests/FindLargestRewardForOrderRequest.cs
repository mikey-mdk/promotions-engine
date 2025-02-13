using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Engines.RewardsEngines.Requests;

[ExcludeFromCodeCoverage]
public class FindLargestRewardForOrderRequest
{
    public List<Promotion> Promotions { get; set; } = new();

    public string OrderId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}