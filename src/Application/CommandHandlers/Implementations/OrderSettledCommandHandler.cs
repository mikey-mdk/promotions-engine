using PromotionsEngine.Application.CommandHandlers.Interfaces;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.CommandHandlers.Implementations;

public class OrderSettledCommandHandler : IOrderSettledCommandHandler
{
    private readonly ICustomerOrderRewardsLedgerRepository _customerOrderRewardsLedgerRepository;
    private readonly IRewardsDistributionReconciliationService _rewardsDistributionReconciliationService;


    public OrderSettledCommandHandler(
        ICustomerOrderRewardsLedgerRepository customerOrderRewardsLedgerRepository,
        IRewardsDistributionReconciliationService rewardsDistributionReconciliationService)
    {
        _customerOrderRewardsLedgerRepository = customerOrderRewardsLedgerRepository;
        _rewardsDistributionReconciliationService = rewardsDistributionReconciliationService;
    }

    public async Task HandleOrderSettledCommand(OrderSettledCommand command, CancellationToken cancellationToken)
    {
        var ledger = await _customerOrderRewardsLedgerRepository.GetLedgerForOrder(command.OrderId, cancellationToken);

        // There may not be a ledger if this order was not eligible for a reward, or the order predates this application.
        if (string.IsNullOrEmpty(ledger?.OrderId))
        {
            return;
        }

        // Idempotency check: there should be a single settled event per purchase-ledger.
        if (ledger.RewardTransactions.Exists(x => x.TransactionType == CTransactionType.OrderSettled))
        {
            return;
        }

        var orderAmountByMerchant = command.TransactionDetails
            .GroupBy(x => x.MerchantName)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalGatewayCaptured - y.TotalGatewayRefunded));

        var shouldSettle = orderAmountByMerchant.Count == 1
            && orderAmountByMerchant.ContainsKey(ledger.Merchant.MerchantName);

        await _rewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync(
            new ReconcileOrderSettledRequest
            {
                ShouldSettle = shouldSettle,
                Command = command,
                Ledger = ledger
            }, cancellationToken);
    }
}
