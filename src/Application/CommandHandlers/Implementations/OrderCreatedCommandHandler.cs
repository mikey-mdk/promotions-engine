using PromotionsEngine.Application.CommandHandlers.Interfaces;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Exceptions;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.CommandHandlers.Implementations;

public class OrderCreatedCommandHandler : IOrderCreatedCommandHandler
{
    private readonly ICustomerOrderRewardsLedgerRepository _customerOrderRewardsLedgerRepository;
    private readonly IPromotionsRepository _promotionsRepository;
    private readonly IPromotionRulesEngine _promotionRulesEngineEvaluator;
    private readonly IRewardsCalculationEngine _rewardsCalculationEngine;
    private readonly IMerchantIdentificationService _merchantIdentificationService;
    private readonly IRewardsDistributionReconciliationService _rewardsDistributionReconciliationService;

    public OrderCreatedCommandHandler(
        ICustomerOrderRewardsLedgerRepository customerOrderRewardsLedgerRepository,
        IPromotionsRepository promotionsRepository,
        IPromotionRulesEngine promotionRulesEngineEvaluator,
        IRewardsCalculationEngine rewardsCalculationEngine,
        IMerchantIdentificationService merchantIdentificationService,
        IRewardsDistributionReconciliationService rewardsDistributionReconciliationService)
    {
        _customerOrderRewardsLedgerRepository = customerOrderRewardsLedgerRepository;
        _promotionsRepository = promotionsRepository;
        _promotionRulesEngineEvaluator = promotionRulesEngineEvaluator;
        _rewardsCalculationEngine = rewardsCalculationEngine;
        _merchantIdentificationService = merchantIdentificationService;
        _rewardsDistributionReconciliationService = rewardsDistributionReconciliationService;
    }

    public async Task HandleOrderCreatedCommand(OrderCreatedCommand command, CancellationToken cancellationToken)
    {
        var customerOrderRewardsLedger =
            await _customerOrderRewardsLedgerRepository.GetLedgerForOrder(command.OrderId, cancellationToken);

        //Idempotency check
        if (customerOrderRewardsLedger != null && !string.IsNullOrEmpty(customerOrderRewardsLedger.OrderId)
            && customerOrderRewardsLedger.RewardTransactions
                .Exists(x => x.TransactionType == CTransactionType.OrderCreated))
        {
            //We've already processed this event type for this order. Do nothing.
            return;
        }

        var merchant = await _merchantIdentificationService.IdentifyMerchantByRegexAsync(command.MerchantName, cancellationToken)
                       ?? throw new DomainObjectNullException("Unable to find merchant for order.");

        var promotions = await _promotionsRepository.GetPromotionsByMerchantIdAsync(merchant.MerchantId, cancellationToken);

        var validPromotionsByPromotionsSummaryList = await _promotionRulesEngineEvaluator.FindValidPromotions(new FindValidPromotionsRequest
        {
            Promotions = promotions,
            OrderAmount = command.OrderAmount,
            CustomerId = command.CustomerId,
            EvaluationContext = CPromotionRuleEvaluationContext.OrderCreated
        });

        var validPromotions = validPromotionsByPromotionsSummaryList.Select(x => x.promotion).ToList();

        var largestReward = await _rewardsCalculationEngine.FindLargestRewardForOrderAsync(
            new FindLargestRewardForOrderRequest
            {
                OrderId = command.OrderId,
                CustomerId = command.CustomerId,
                Amount = command.OrderAmount,
                Promotions = validPromotions
            });

        await _rewardsDistributionReconciliationService.PerformOrderCreatedReconciliationAsync(
            new ReconcileOrderCreatedRequest
            {
                Command = command,
                Promotion = validPromotions.Single(x => x.Id == largestReward.PromotionId),
                PromotionSummary = validPromotionsByPromotionsSummaryList
                    .Single(x => x.promotionSummary.Id == largestReward.PromotionId).promotionSummary,
                RewardAmount = largestReward.Amount,
                Merchant = merchant
            }, cancellationToken);
    }
}