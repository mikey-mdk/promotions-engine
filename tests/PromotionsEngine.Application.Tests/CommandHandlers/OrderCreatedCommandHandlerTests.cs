using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.CommandHandlers.Implementations;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Exceptions;
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
public class OrderCreatedCommandHandlerTests
{
    private readonly ICustomerOrderRewardsLedgerRepository _fakeCustomerOrderRewardsLedgerRepository;
    private readonly IPromotionsRepository _fakePromotionsRepository;
    private readonly IPromotionRulesEngine _fakePromotionRulesEngine;
    private readonly IRewardsCalculationEngine _fakeRewardsCalculationEngine;
    private readonly IMerchantIdentificationService _fakeMerchantIdentificationService;
    private readonly IRewardsDistributionReconciliationService _fakeRewardsDistributionReconciliationService;

    private readonly OrderCreatedCommandHandler _orderCreatedCommandHandler;

    public OrderCreatedCommandHandlerTests()
    {
        _fakeCustomerOrderRewardsLedgerRepository = A.Fake<ICustomerOrderRewardsLedgerRepository>();
        _fakePromotionsRepository = A.Fake<IPromotionsRepository>();
        _fakePromotionRulesEngine = A.Fake<IPromotionRulesEngine>();
        _fakeRewardsCalculationEngine = A.Fake<IRewardsCalculationEngine>();
        _fakeMerchantIdentificationService = A.Fake<IMerchantIdentificationService>();
        _fakeRewardsDistributionReconciliationService = A.Fake<IRewardsDistributionReconciliationService>();

        _orderCreatedCommandHandler = new OrderCreatedCommandHandler(
            _fakeCustomerOrderRewardsLedgerRepository,
            _fakePromotionsRepository,
            _fakePromotionRulesEngine,
            _fakeRewardsCalculationEngine,
            _fakeMerchantIdentificationService,
            _fakeRewardsDistributionReconciliationService);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(OrderCreatedCommandHandler))]
    [Trait("Method", "HandleOrderCreatedCommand")]
    [Description($"Test the happy path for {nameof(OrderCreatedCommandHandler)}")]
    public async Task Test_Handle_Order_Created_Happy_Path()
    {
        var orderId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();
        var merchantId = Guid.NewGuid().ToString();
        var promotionId = Guid.NewGuid().ToString();
        var rewardAmount = 10m;

        var ledgerRepositoryGetConfiguration = A
            .CallTo(() =>
                _fakeCustomerOrderRewardsLedgerRepository.GetLedgerForOrder(A<string>.That.Matches(x => x == orderId), A<CancellationToken>._));
        ledgerRepositoryGetConfiguration.Returns(new CustomerOrderRewardsLedger());

        var identificationCall = A.CallTo(() =>
            _fakeMerchantIdentificationService.IdentifyMerchantByRegexAsync(A<string>._, A<CancellationToken>._));
        identificationCall.Returns(new Merchant { MerchantId = merchantId });

        var promotionRepositoryGetConfiguration = A.CallTo(() =>
            _fakePromotionsRepository.GetPromotionsByMerchantIdAsync(A<string>.That.Matches(x => x == merchantId), default));
        promotionRepositoryGetConfiguration.Returns(new List<Promotion>());

        var promotionRulesConfig =
            A.CallTo(() => _fakePromotionRulesEngine.FindValidPromotions(A<FindValidPromotionsRequest>._));
        promotionRulesConfig.Returns(new List<(Promotion promotion, PromotionSummary promotionSummary)>
        {
            new ()
            {
                promotion = new Promotion
                {
                    Id = promotionId
                },
                promotionSummary = new PromotionSummary
                {
                    Id = promotionId
                }
            }
        });

        var rewardsCalculationEngineConfig = A.CallTo(() =>
            _fakeRewardsCalculationEngine.FindLargestRewardForOrderAsync(A<FindLargestRewardForOrderRequest>._));
        rewardsCalculationEngineConfig.Returns(new Reward
        {
            PromotionId = promotionId,
            Amount = rewardAmount
        });

        await _orderCreatedCommandHandler.HandleOrderCreatedCommand(new OrderCreatedCommand
        {
            OrderId = orderId,
            CustomerId = customerId,
            ExternalMerchantId = merchantId
        }, default);

        ledgerRepositoryGetConfiguration.MustHaveHappenedOnceExactly();
        identificationCall.MustHaveHappenedOnceExactly();
        promotionRulesConfig.MustHaveHappenedOnceExactly();
        rewardsCalculationEngineConfig.MustHaveHappenedOnceExactly();
        promotionRepositoryGetConfiguration.MustHaveHappenedOnceExactly();

        A.CallTo(() =>
            _fakeRewardsDistributionReconciliationService.PerformOrderCreatedReconciliationAsync(
                A<ReconcileOrderCreatedRequest>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(OrderCreatedCommandHandler))]
    [Trait("Method", "HandleOrderCreatedCommand")]
    [Description($"Test the idempotency of {nameof(OrderCreatedCommandHandler)}")]
    public async Task Test_Handle_Order_Created_Idempotency()
    {
        var orderId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();

        var customerOrderRewardsLedger = new CustomerOrderRewardsLedger
        {
            OrderId = orderId,
            CustomerId = customerId,
            Merchant = new Merchant(),
            Promotion = new Promotion(),
            RewardBalance = 100m,
            RewardTransactions = new List<RewardTransaction>
            {
                new()
                {
                    TransactionType = CTransactionType.OrderCreated
                }
            }
        };

        var ledgerRepositoryGetConfiguration = A
            .CallTo(() =>
                _fakeCustomerOrderRewardsLedgerRepository.GetLedgerForOrder(A<string>.That.Matches(x => x == orderId),
                    A<CancellationToken>._));
        ledgerRepositoryGetConfiguration.Returns(customerOrderRewardsLedger);

        await _orderCreatedCommandHandler.HandleOrderCreatedCommand(new OrderCreatedCommand
        { CustomerId = customerId, OrderId = orderId }, default);

        ledgerRepositoryGetConfiguration.MustHaveHappenedOnceExactly();
        A.CallTo(() =>
                _fakeMerchantIdentificationService.IdentifyMerchantByRegexAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() =>
            _fakePromotionsRepository.GetPromotionsByMerchantIdAsync(A<string>._, default)).MustNotHaveHappened();
        A.CallTo(() => _fakePromotionRulesEngine.FindValidPromotions(A<FindValidPromotionsRequest>._))
            .MustNotHaveHappened();
        A.CallTo(() =>
            _fakeRewardsCalculationEngine.FindLargestRewardForOrderAsync(A<FindLargestRewardForOrderRequest>._)).MustNotHaveHappened();
        A.CallTo(() =>
            _fakeRewardsDistributionReconciliationService.PerformOrderCreatedReconciliationAsync(
                A<ReconcileOrderCreatedRequest>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(OrderCreatedCommandHandler))]
    [Trait("Method", "HandleOrderCreatedCommand")]
    [Description($"Test that exception is thrown is merchant is null {nameof(OrderCreatedCommandHandler)}")]
    public async Task Test_Exception_Occurs_When_Merchant_Is_Null()
    {
        var orderId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();
        var merchantId = Guid.NewGuid().ToString();

        var customerOrderRewardsLedger = new CustomerOrderRewardsLedger
        {
            OrderId = orderId,
            CustomerId = customerId,
            Merchant = new Merchant(),
            Promotion = new Promotion(),
            RewardBalance = 100m,
        };

        var ledgerRepositoryGetConfiguration = A
        .CallTo(() => _fakeCustomerOrderRewardsLedgerRepository.GetLedgerForOrder(A<string>.That.Matches(x => x == orderId), A<CancellationToken>._));
        ledgerRepositoryGetConfiguration.Returns(customerOrderRewardsLedger);

        A.CallTo(() =>
                _fakeMerchantIdentificationService.IdentifyMerchantByRegexAsync(A<string>._, A<CancellationToken>._))
            .Returns((Merchant?)null);

        await Assert.ThrowsAsync<DomainObjectNullException>(() => _orderCreatedCommandHandler.HandleOrderCreatedCommand(new OrderCreatedCommand
        {
            OrderId = orderId,
            CustomerId = customerId,
            ExternalMerchantId = merchantId
        }, default));
    }
}