using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Requests.Reconciliation;

[ExcludeFromCodeCoverage]
public class ReconcileOrderSettledRequest
{
    public bool ShouldSettle { get; set; }

    public CustomerOrderRewardsLedger Ledger { get; set; } = new();

    public OrderSettledCommand Command { get; set; } = new();
}