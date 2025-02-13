using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.CommandHandlers.Implementations;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Application.CommandHandlers;

[ExcludeFromCodeCoverage]
public class OrderRefundedCommandHandlerTests
{
    private readonly ICustomerOrderRewardsLedgerRepository _fakeCustomerOrderRewardsLedgerRepository;
    private readonly IPromotionSummaryRepository _fakePromotionSummaryRepository;
    private readonly IRewardsCalculationEngine _fakeRewardsCalculationEngine;
    private readonly ILogger<OrderRefundedCommandHandler> _fakeLogger;
    private readonly IPromotionRulesEngine _fakePromotionRulesEngine;
    private readonly IRewardsDistributionReconciliationService _fakeRewardsDistributionReconciliationService;

    private readonly OrderRefundedCommandHandler _orderRefundedCommandHandler;

    public OrderRefundedCommandHandlerTests()
    {
        _fakeCustomerOrderRewardsLedgerRepository = A.Fake<ICustomerOrderRewardsLedgerRepository>();
        _fakePromotionSummaryRepository = A.Fake<IPromotionSummaryRepository>();
        _fakeRewardsCalculationEngine = A.Fake<IRewardsCalculationEngine>();
        _fakeLogger = A.Fake<ILogger<OrderRefundedCommandHandler>>();
        _fakePromotionRulesEngine = A.Fake<IPromotionRulesEngine>();
        _fakeRewardsDistributionReconciliationService = A.Fake<IRewardsDistributionReconciliationService>();

        _orderRefundedCommandHandler = new OrderRefundedCommandHandler(
            _fakeCustomerOrderRewardsLedgerRepository,
            _fakeLogger,
            _fakePromotionSummaryRepository,
            _fakeRewardsCalculationEngine,
            _fakePromotionRulesEngine,
            _fakeRewardsDistributionReconciliationService);
    }

    [Fact]
    [Trait("Class", nameof(OrderRefundedCommandHandler))]
    [Description("Test that when the partial refund is no longer valid that a full refund is issued")]
    public async Task Should_Send_Reward_Difference_When_New_Promotion_Amount_Is_Invalid()
    {
        const string promotionId = "promotion-id";
        const decimal refundAmount = 10m;
        const int numberOfTimesRedeemed = 10;
        const int totalNumberOfCustomers = 10;
        const decimal totalAmountRedeemed = 1000m;
        const decimal rewardBalance = 100m;

        // Arrange
        var command = new OrderRefundedCommand
        {
            OrderId = "order-id",
            CustomerId = "customer-id",
            ExternalMerchantId = "merchant-id",
            Amount = refundAmount
        };

        var promotionResponse = new Promotion
        {
            Id = promotionId
        };

        var ledgerResponse = new CustomerOrderRewardsLedger
        {
            OrderId = command.OrderId,
            CustomerId = command.CustomerId,
            RewardBalance = rewardBalance,
            Promotion = promotionResponse
        };

        var promotionSummary = new PromotionSummary
        {
            Id = promotionId,
            NumberOfTimesRedeemed = numberOfTimesRedeemed,
            TotalNumberOfCustomers = totalNumberOfCustomers,
            TotalAmountRedeemed = totalAmountRedeemed
        };

        var ledgerRepositoryGetConfiguration = A.CallTo(() => _fakeCustomerOrderRewardsLedgerRepository.GetLedgerForOrder(command.OrderId, A<CancellationToken>._));
        ledgerRepositoryGetConfiguration.Returns(ledgerResponse);

        var rulesEngineConfig =
            A.CallTo(() => _fakePromotionRulesEngine.FindValidPromotions(A<FindValidPromotionsRequest>._));
        rulesEngineConfig.Returns(new List<(Promotion promotion, PromotionSummary promotionSummary)>());

        var promotionSummaryGetConfig = A.CallTo(() => _fakePromotionSummaryRepository.GetPromotionSummaryAsync(promotionId, default));
        promotionSummaryGetConfig.Returns(promotionSummary);

        // Act
        await _orderRefundedCommandHandler.HandleOrderRefundedCommand(command, default);

        // Assert
        ledgerRepositoryGetConfiguration.MustHaveHappenedOnceExactly();
        rulesEngineConfig.MustHaveHappenedOnceExactly();
        promotionSummaryGetConfig.MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakeRewardsCalculationEngine.CalculateRewardsAsync(A<RewardsCalculationRequest>._)).MustNotHaveHappened();

        A.CallTo(() =>
            _fakeRewardsDistributionReconciliationService.PerformOrderRefundedReconciliationAsync(
                A<ReconcileOrderRefundedRequest>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Class", nameof(OrderRefundedCommandHandler))]
    [Description("Test that when the partial refund is still valid that the refund amount is issued")]
    public async Task Should_send_reward_difference_on_partial_refund()
    {
        const string promotionId = "promotion-id";
        const decimal refundAmount = 10m;
        const int numberOfTimesRedeemed = 10;
        const int totalNumberOfCustomers = 10;
        const decimal totalAmountRedeemed = 1000m;
        const decimal rewardBalance = 100m;
        const decimal newRewardBalance = 90m;

        // Arrange
        var command = new OrderRefundedCommand
        {
            OrderId = "order-id",
            CustomerId = "customer-id",
            ExternalMerchantId = "merchant-id",
            Amount = refundAmount
        };

        var promotionResponse = new Promotion
        {
            Id = promotionId
        };

        var ledgerResponse = new CustomerOrderRewardsLedger
        {
            OrderId = command.OrderId,
            CustomerId = command.CustomerId,
            RewardBalance = rewardBalance,
            Promotion = promotionResponse,
            RewardTransactions = new List<RewardTransaction>
            {
                new()
                {
                    TransactionType = CTransactionType.OrderCreated,
                    Amount = 100
                }
            }
        };

        var promotionSummary = new PromotionSummary
        {
            Id = promotionId,
            NumberOfTimesRedeemed = numberOfTimesRedeemed,
            TotalNumberOfCustomers = totalNumberOfCustomers,
            TotalAmountRedeemed = totalAmountRedeemed
        };

        var promotionByPromotionSummary = ValueTuple.Create(promotionResponse, promotionSummary);

        var ledgerRepositoryGetConfiguration = A.CallTo(() => _fakeCustomerOrderRewardsLedgerRepository.GetLedgerForOrder(command.OrderId, A<CancellationToken>._));
        ledgerRepositoryGetConfiguration.Returns(ledgerResponse);

        var rulesEngineConfig =
            A.CallTo(() => _fakePromotionRulesEngine.FindValidPromotions(A<FindValidPromotionsRequest>._));
        rulesEngineConfig.Returns(new List<(Promotion promotion, PromotionSummary promotionSummary)>
        {
            promotionByPromotionSummary
        });

        var rewardsCalculationEngineCall = A.CallTo(() => _fakeRewardsCalculationEngine.CalculateRewardsAsync(A<RewardsCalculationRequest>._));
        rewardsCalculationEngineCall.Returns(newRewardBalance);

        // Act
        await _orderRefundedCommandHandler.HandleOrderRefundedCommand(command, default);

        // Assert
        ledgerRepositoryGetConfiguration.MustHaveHappenedOnceExactly();
        rulesEngineConfig.MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakePromotionSummaryRepository.GetPromotionSummaryAsync(promotionId, default)).MustNotHaveHappened();

        rewardsCalculationEngineCall.MustHaveHappenedOnceExactly();

        A.CallTo(() =>
            _fakeRewardsDistributionReconciliationService.PerformOrderRefundedReconciliationAsync(
                A<ReconcileOrderRefundedRequest>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    //test idempotency for missing ledger

    //test idempotency for ledger transaction type

    //test newOrderAmount < 0

    //test rewardDifference = 0

    //test customer list is update on full refund
}
