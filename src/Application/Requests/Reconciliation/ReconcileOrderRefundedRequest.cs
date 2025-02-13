using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Requests.Reconciliation;

[ExcludeFromCodeCoverage]
public class ReconcileOrderRefundedRequest
{
    public Promotion Promotion { get; set; } = new();

    public PromotionSummary PromotionSummary { get; set; } = new();

    public CustomerOrderRewardsLedger CustomerOrderRewardsLedger { get; set; } = new();

    public OrderRefundedCommand Command { get; set; } = new();

    public string CustomerId { get; set; } = string.Empty;

    public decimal NewRewardBalance { get; set; }

    public decimal RewardDifference { get; set; }
}