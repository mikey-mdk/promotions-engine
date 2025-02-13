using Microsoft.Extensions.Logging;
using PromotionsEngine.Application.CommandHandlers.Interfaces;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Exceptions;
using PromotionsEngine.Application.Extensions;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.CommandHandlers.Implementations;

public class OrderRefundedCommandHandler : IOrderRefundedCommandHandler
{
    private readonly ICustomerOrderRewardsLedgerRepository _customerOrderRewardsLedgerRepository;
    private readonly IPromotionSummaryRepository _promotionsSummaryRepository;
    private readonly IRewardsCalculationEngine _rewardsCalculationEngine;
    private readonly ILogger<OrderRefundedCommandHandler> _logger;
    private readonly IPromotionRulesEngine _promotionRulesEngine;
    private readonly IRewardsDistributionReconciliationService _rewardsDistributionReconciliationService;

    public OrderRefundedCommandHandler(
        ICustomerOrderRewardsLedgerRepository customerOrderRewardsLedgerRepository,
        ILogger<OrderRefundedCommandHandler> logger,
        IPromotionSummaryRepository promotionsSummaryRepository,
        IRewardsCalculationEngine rewardsCalculationEngine,
        IPromotionRulesEngine promotionRulesEngine,
        IRewardsDistributionReconciliationService rewardsDistributionReconciliationService)
    {
        _customerOrderRewardsLedgerRepository = customerOrderRewardsLedgerRepository;
        _logger = logger;
        _promotionsSummaryRepository = promotionsSummaryRepository;
        _rewardsCalculationEngine = rewardsCalculationEngine;
        _promotionRulesEngine = promotionRulesEngine;
        _rewardsDistributionReconciliationService = rewardsDistributionReconciliationService;
    }

    public async Task HandleOrderRefundedCommand(OrderRefundedCommand command, CancellationToken cancellationToken)
    {
        var customerOrderRewardsLedger =
            await _customerOrderRewardsLedgerRepository.GetLedgerForOrder(command.OrderId, cancellationToken);

        // There may not be a ledger if this order was not eligible for a reward, or the order predates this application.
        if (customerOrderRewardsLedger == null || string.IsNullOrEmpty(customerOrderRewardsLedger.OrderId))
        {
            return;
        }

        // Idempotency check. If we have already processed this order-refunded event it will be logged in the reward-transactions collection.
        if (customerOrderRewardsLedger.RewardTransactions
            .Exists(x => x.TransactionType == CTransactionType.OrderRefunded && x.TransactionId == command.TransactionId))
        {
            return;
        }

        //We should use the promotion attached to the ledger
        var promotion = customerOrderRewardsLedger.Promotion;

        customerOrderRewardsLedger.RewardTransactions.Add(new RewardTransaction
        {
            TransactionType = CTransactionType.OrderRefunded,
            TransactionId = command.TransactionId,
            OrderId = command.OrderId,
            Amount = command.Amount
        });

        var newOrderAmount = customerOrderRewardsLedger.RewardTransactions.CalculateOrderAmount();

        if (newOrderAmount < 0)
        {
            _logger.LogWarning("Calculated an order amount less than 0. Amount: {amount}, Order: {orderId}", newOrderAmount, command.OrderId);
            newOrderAmount = 0;
        }

        var promotionsByPromotionsSummaryList = await _promotionRulesEngine.FindValidPromotions(
            new FindValidPromotionsRequest
            {
                Promotions = new List<Promotion> { promotion },
                OrderAmount = newOrderAmount,
                EvaluationContext = CPromotionRuleEvaluationContext.OrderRefunded
            });

        decimal newRewardBalance;
        decimal rewardDifference;
        //The change in amount after the refund made the promotion no longer valid so we will issue a full refund.
        if (promotionsByPromotionsSummaryList.Count == 0)
        {
            newRewardBalance = 0;
            rewardDifference = customerOrderRewardsLedger.RewardBalance;

            var promotionSummary =
                await _promotionsSummaryRepository.GetPromotionSummaryAsync(promotion.Id, cancellationToken);

            if (promotionSummary == null)
            {
                throw new DomainObjectNullException(nameof(promotionSummary));
            }

            await _rewardsDistributionReconciliationService.PerformOrderRefundedReconciliationAsync(
                new ReconcileOrderRefundedRequest
                {
                    Promotion = promotion,
                    PromotionSummary = promotionSummary,
                    Command = command,
                    CustomerOrderRewardsLedger = customerOrderRewardsLedger,
                    NewRewardBalance = newRewardBalance,
                    RewardDifference = rewardDifference,
                }, cancellationToken);
        }
        else //Current promotion is still valid after the refund
        {
            newRewardBalance = await _rewardsCalculationEngine.CalculateRewardsAsync(new RewardsCalculationRequest
            {
                OrderAmount = newOrderAmount,
                RewardRateType = promotion.RewardRateTypeEnum!,
                RateAmount = promotion.RateAmount
            });

            rewardDifference = customerOrderRewardsLedger.RewardBalance - newRewardBalance;

            if (rewardDifference == 0)
            {
                _logger.LogWarning("Received a refund event but calculated 0 reward difference. OrderId: {orderId} ", command.OrderId);
            }
            else
            {
                var promotionSummary = promotionsByPromotionsSummaryList
                    .Single(x => x.promotionSummary.Id == promotion.Id).promotionSummary;

                await _rewardsDistributionReconciliationService.PerformOrderRefundedReconciliationAsync(
                    new ReconcileOrderRefundedRequest
                    {
                        Promotion = promotion,
                        PromotionSummary = promotionSummary,
                        Command = command,
                        CustomerOrderRewardsLedger = customerOrderRewardsLedger,
                        NewRewardBalance = newRewardBalance,
                        RewardDifference = rewardDifference,
                    }, cancellationToken);
            }
        }
    }
}
