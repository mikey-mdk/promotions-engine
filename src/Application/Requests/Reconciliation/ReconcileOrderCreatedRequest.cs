using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Requests.Reconciliation;

[ExcludeFromCodeCoverage]
public class ReconcileOrderCreatedRequest
{
    public Domain.Models.Merchant Merchant { get; set; } = new();

    public Promotion Promotion { get; set; } = new();

    public PromotionSummary PromotionSummary { get; set; } = new();

    public OrderCreatedCommand Command { get; set; } = new();

    public decimal RewardAmount { get; set; }
}