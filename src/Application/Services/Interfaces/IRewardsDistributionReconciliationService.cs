using PromotionsEngine.Application.Requests.Reconciliation;

namespace PromotionsEngine.Application.Services.Interfaces;

public interface IRewardsDistributionReconciliationService
{
    /// <summary>
    /// Performs the post reward distribution reconciliation logic for the order created event.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PerformOrderCreatedReconciliationAsync(ReconcileOrderCreatedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Performs the post reward distribution reconciliation logic for the order refunded event.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PerformOrderRefundedReconciliationAsync(ReconcileOrderRefundedRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Performs the post reward distribution reconciliation logic for the order settled event.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PerformOrderSettledReconciliationAsync(ReconcileOrderSettledRequest request, CancellationToken cancellationToken);
}