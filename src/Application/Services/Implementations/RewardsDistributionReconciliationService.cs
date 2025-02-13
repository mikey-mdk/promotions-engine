using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromotionsEngine.Application.BusMessages;
using PromotionsEngine.Application.Configuration;
using PromotionsEngine.Application.Exceptions;
using PromotionsEngine.Application.Managers.Interfaces;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.Services.Implementations;

public class RewardsDistributionReconciliationService : IRewardsDistributionReconciliationService
{
    private readonly IPromotionsRepository _promotionsRepository;
    private readonly IPromotionSummaryRepository _promotionSummaryRepository;
    private readonly ICustomerOrderRewardsLedgerRepository _customerOrderRewardsLedgerRepository;
    private readonly IServiceBusManager _serviceBusManager;
    private readonly ServiceBusOptions _serviceBusOptions;
    private readonly ILogger<RewardsDistributionReconciliationService> _logger;

    public RewardsDistributionReconciliationService(
        IPromotionsRepository promotionsRepository,
        IPromotionSummaryRepository promotionSummaryRepository,
        ICustomerOrderRewardsLedgerRepository customerOrderRewardsLedgerRepository,
        IServiceBusManager serviceBusManager,
        IOptions<ServiceBusOptions> serviceBusOptions,
        ILogger<RewardsDistributionReconciliationService> logger)
    {
        _promotionsRepository = promotionsRepository;
        _promotionSummaryRepository = promotionSummaryRepository;
        _customerOrderRewardsLedgerRepository = customerOrderRewardsLedgerRepository;
        _serviceBusManager = serviceBusManager;
        _logger = logger;
        _serviceBusOptions = serviceBusOptions.Value;
    }

    public async Task PerformOrderCreatedReconciliationAsync(ReconcileOrderCreatedRequest request, CancellationToken cancellationToken)
    {
        request.PromotionSummary.NumberOfTimesRedeemed += 1;
        request.PromotionSummary.TotalAmountRedeemed += request.RewardAmount;
        request.PromotionSummary.TotalNumberOfCustomers += 1;
        request.Promotion.CustomerIds.Add(request.Command.CustomerId);

        var customerOrderRewardsLedger = new CustomerOrderRewardsLedger
        {
            OrderId = request.Command.OrderId,
            CustomerId = request.Command.CustomerId,
            Merchant = request.Merchant,
            Promotion = request.Promotion,
            RewardTransactions =
            {
                new RewardTransaction
                {
                    OrderId = request.Command.OrderId,
                    AuthorizationId = request.Command.AuthorizationId,
                    MerchantId = request.Command.ExternalMerchantId,
                    MerchantName = request.Command.MerchantName,
                    TransactionType = CTransactionType.OrderCreated,
                    Amount = request.Command.OrderAmount
                }
            }
        };

        await _promotionsRepository.UpdatePromotionAsync(request.Promotion, cancellationToken);
        await _promotionSummaryRepository.UpdatePromotionSummaryAsync(request.PromotionSummary, cancellationToken);
        await _customerOrderRewardsLedgerRepository.CreateCustomerOrderRewardsLedger(customerOrderRewardsLedger, cancellationToken);

        await SendServiceBusMessage(request.RewardAmount, request.Command.CustomerId, request.Command.OrderId,
            CTransactionType.OrderCreated, cancellationToken);
    }

    public async Task PerformOrderRefundedReconciliationAsync(ReconcileOrderRefundedRequest request,
        CancellationToken cancellationToken)
    {
        request.CustomerOrderRewardsLedger.RewardBalance = request.NewRewardBalance;
        await _customerOrderRewardsLedgerRepository.UpdateCustomerOrderRewardsLedger(request.CustomerOrderRewardsLedger, cancellationToken);

        var isFullRefund = request.NewRewardBalance <= 0;

        await UpdatePromotionInfo(request.Promotion, request.PromotionSummary, isFullRefund, request.RewardDifference, request.CustomerId, cancellationToken);
        await SendServiceBusMessage(request.RewardDifference, request.CustomerId, request.CustomerOrderRewardsLedger.OrderId,
            CTransactionType.OrderRefunded, cancellationToken);
    }

    public async Task PerformOrderSettledReconciliationAsync(ReconcileOrderSettledRequest request, CancellationToken cancellationToken)
    {
        if (request.ShouldSettle)
        {
            request.Ledger.RewardTransactions.Add(RewardTransaction.Settled(request.Command.OrderId, 0));
            await _customerOrderRewardsLedgerRepository.UpdateCustomerOrderRewardsLedger(request.Ledger, cancellationToken);
            await SendServiceBusMessage(0, request.Ledger.CustomerId,
                request.Ledger.OrderId, CTransactionType.OrderSettled, cancellationToken);
        }
        else
        {
            var rewardDifference = -request.Ledger.RewardBalance;

            request.Ledger.RewardBalance = 0;
            request.Ledger.RewardTransactions.Add(RewardTransaction.Settled(request.Command.OrderId, 0));
            await _customerOrderRewardsLedgerRepository.UpdateCustomerOrderRewardsLedger(request.Ledger, cancellationToken);

            await SendServiceBusMessage(rewardDifference, request.Ledger.CustomerId,
                request.Ledger.OrderId, CTransactionType.OrderSettled, cancellationToken);
            await RewardDisqualified(request.Ledger, rewardDifference, cancellationToken);
        }
    }

    private async Task SendServiceBusMessage(decimal rewardAmount, string customerId, string orderId, string transactionType, CancellationToken cancellationToken)
    {
        switch (transactionType)
        {
            case CTransactionType.OrderCreated:
                await _serviceBusManager.SendMessageToServiceBus(
                    PromotionsEngineBalanceUpdateMessage.Created(rewardAmount, orderId, customerId),
                    _serviceBusOptions.PromotionsEngineBalanceUpdateQueueName, cancellationToken);
                break;
            case CTransactionType.OrderRefunded:
                await _serviceBusManager.SendMessageToServiceBus(
                    PromotionsEngineBalanceUpdateMessage.Refunded(rewardAmount, orderId, customerId),
                    _serviceBusOptions.PromotionsEngineBalanceUpdateQueueName, cancellationToken);
                break;
            case CTransactionType.OrderSettled:
                await _serviceBusManager.SendMessageToServiceBus(
                    PromotionsEngineBalanceUpdateMessage.Settled(rewardAmount, orderId, customerId),
                    _serviceBusOptions.PromotionsEngineBalanceUpdateQueueName, cancellationToken);
                break;
            default:
                _logger.LogWarning("Unknown transaction type: {transactionType}", transactionType);
                break;
        }
    }

    /// <summary>
    /// Update the promotion summary totals.
    /// </summary>
    private async Task UpdatePromotionInfo(Promotion promotion, PromotionSummary promotionSummary, bool isFullRefund,
        decimal rewardDifference, string customerId, CancellationToken cancellationToken)
    {
        promotionSummary.TotalAmountRedeemed -= rewardDifference;

        if (isFullRefund)
        {
            promotionSummary.NumberOfTimesRedeemed -= 1;
            promotionSummary.TotalNumberOfCustomers -= 1;

            promotion.CustomerIds.Remove(customerId);
            await _promotionsRepository.UpdatePromotionAsync(promotion, cancellationToken);
        }

        await _promotionSummaryRepository.UpdatePromotionSummaryAsync(promotionSummary, cancellationToken);
    }

    /// <summary>
    /// Update the state of the promotion and promotion-summary when a reward ledger is disqualified.
    /// </summary>
    /// <remarks>
    /// When a reward is disqualified we should remove the state associated with it so another customer can qualify.
    /// </remarks>
    private async Task RewardDisqualified(
        CustomerOrderRewardsLedger ledger,
        decimal balanceChange,
        CancellationToken cancellationToken)
    {
        var promotionSummary = await _promotionSummaryRepository.GetPromotionSummaryAsync(ledger.Promotion.Id, cancellationToken)
                               ?? throw new DomainObjectNullException($"Failed to get promotion summary: {ledger.Promotion.Id}");

        promotionSummary.TotalAmountRedeemed += balanceChange;
        promotionSummary.NumberOfTimesRedeemed -= 1;
        promotionSummary.TotalNumberOfCustomers -= 1;
        await _promotionSummaryRepository.UpdatePromotionSummaryAsync(promotionSummary, cancellationToken);

        var promotion = await _promotionsRepository.GetPromotionByIdAsync(ledger.Promotion.Id, cancellationToken)
                        ?? throw new DomainObjectNullException($"Failed to get promotion: {ledger.Promotion.Id}");

        promotion.CustomerIds.Remove(ledger.CustomerId);
        await _promotionsRepository.UpdatePromotionAsync(promotion, cancellationToken);
    }
}