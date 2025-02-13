using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromotionsEngine.Application.BusMessages;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Configuration;
using PromotionsEngine.Application.Managers.Interfaces;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Implementations;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Application.Services;

public class RewardsDistributionReconciliationServiceTests
{
    private readonly IPromotionsRepository _fakePromotionsRepository;
    private readonly IPromotionSummaryRepository _fakePromotionSummaryRepository;
    private readonly ICustomerOrderRewardsLedgerRepository _fakeCustomerOrderRewardsLedgerRepository;
    private readonly IServiceBusManager _fakeServiceBusManager;
    private readonly IOptions<ServiceBusOptions> _serviceBusOptions;
    private readonly ILogger<RewardsDistributionReconciliationService> _fakeLogger;

    private readonly IFixture _fixture;

    private readonly IRewardsDistributionReconciliationService _service;

    public RewardsDistributionReconciliationServiceTests()
    {
        _fakePromotionsRepository = A.Fake<IPromotionsRepository>();
        _fakePromotionSummaryRepository = A.Fake<IPromotionSummaryRepository>();
        _fakeCustomerOrderRewardsLedgerRepository = A.Fake<ICustomerOrderRewardsLedgerRepository>();
        _fakeServiceBusManager = A.Fake<IServiceBusManager>();
        _serviceBusOptions = Options.Create(new ServiceBusOptions
        {
            PromotionsEngineBalanceUpdateQueueName = "balanceQueue",
            PromotionsEngineTransactionQueueName = "transactionQueue"
        });
        _fakeLogger = A.Fake<ILogger<RewardsDistributionReconciliationService>>();

        _fixture = new Fixture();

        _service = new RewardsDistributionReconciliationService(
            _fakePromotionsRepository,
            _fakePromotionSummaryRepository,
            _fakeCustomerOrderRewardsLedgerRepository,
            _fakeServiceBusManager,
            _serviceBusOptions,
            _fakeLogger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(RewardsDistributionReconciliationService))]
    [Trait("Method", nameof(RewardsDistributionReconciliationService.PerformOrderCreatedReconciliationAsync))]
    [Description($"Test the happy path for {nameof(RewardsDistributionReconciliationService.PerformOrderCreatedReconciliationAsync)}")]
    public async Task Test_Order_Created_Reconciliation_Success()
    {
        var promotionSummary = A.Fake<PromotionSummary>(x => _fixture.Create<PromotionSummary>());
        var promotion = A.Fake<Promotion>(x => _fixture.Create<Promotion>());
        var command = _fixture.Create<OrderCreatedCommand>();
        var merchant = _fixture.Create<Merchant>();

        var rewardAmount = 100m;

        var request = new ReconcileOrderCreatedRequest
        {
            Promotion = promotion,
            PromotionSummary = promotionSummary,
            Command = command,
            Merchant = merchant,
            RewardAmount = rewardAmount
        };

        var expectedPromotionSummary = promotionSummary;
        expectedPromotionSummary.NumberOfTimesRedeemed += 1;
        expectedPromotionSummary.TotalAmountRedeemed += rewardAmount;
        expectedPromotionSummary.TotalNumberOfCustomers += 1;

        var expectedPromotion = promotion;
        expectedPromotion.CustomerIds.Add(command.CustomerId);

        var expectedLedger = new CustomerOrderRewardsLedger
        {
            OrderId = request.Command.OrderId,
            CustomerId = request.Command.CustomerId,
            Merchant = request.Merchant,
            Promotion = expectedPromotion,
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

        var expectedServiceBusMessage = PromotionsEngineBalanceUpdateMessage.Created(rewardAmount, command.OrderId,
            command.CustomerId);

        await _service.PerformOrderCreatedReconciliationAsync(request, CancellationToken.None);

        A.CallTo(() =>
            _fakePromotionsRepository.UpdatePromotionAsync(A<Promotion>.That.IsEqualTo(expectedPromotion),
                A<CancellationToken>._)).MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakePromotionSummaryRepository.UpdatePromotionSummaryAsync(A<PromotionSummary>.That.IsEqualTo(expectedPromotionSummary), A<CancellationToken>._)).MustHaveHappenedOnceExactly();

        A.CallTo(() =>
                _fakeCustomerOrderRewardsLedgerRepository.CreateCustomerOrderRewardsLedger(
                    A<CustomerOrderRewardsLedger>.That.Matches(x => RewardsLedgersMatch(x, expectedLedger)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() =>
                _fakeServiceBusManager.SendMessageToServiceBus(
                    A<PromotionsEngineBalanceUpdateMessage>.That.Matches(x => ServiceBusMessagesMatch(x, expectedServiceBusMessage)),
                    _serviceBusOptions.Value.PromotionsEngineBalanceUpdateQueueName, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(RewardsDistributionReconciliationService))]
    [Trait("Method", nameof(RewardsDistributionReconciliationService.PerformOrderRefundedReconciliationAsync))]
    [Description($"Test the happy path for {nameof(RewardsDistributionReconciliationService.PerformOrderRefundedReconciliationAsync)}")]
    public async Task Test_Order_Refunded_Reconciliation_Full_Refund_Success()
    {
        var promotionSummary = A.Fake<PromotionSummary>(x => _fixture.Create<PromotionSummary>());
        var promotion = A.Fake<Promotion>(x => _fixture.Create<Promotion>());
        var command = _fixture.Create<OrderRefundedCommand>();
        var ledger = _fixture.Build<CustomerOrderRewardsLedger>().Without(x => x.RewardTransactions).Create();

        var rewardAmount = 100m;
        var newRewardBalance = 0m;
        var rewardDifference = 100m;

        var request = new ReconcileOrderRefundedRequest()
        {
            Promotion = promotion,
            PromotionSummary = promotionSummary,
            Command = command,
            CustomerId = command.CustomerId,
            CustomerOrderRewardsLedger = ledger,
            NewRewardBalance = newRewardBalance,
            RewardDifference = rewardDifference
        };

        var expectedPromotion = promotion;
        expectedPromotion.CustomerIds.Remove(command.CustomerId);

        var expectedLedger = ledger;
        expectedLedger.RewardBalance = newRewardBalance;
        expectedLedger.RewardTransactions = new List<RewardTransaction>
        {
            new()
            {

                TransactionType = CTransactionType.OrderRefunded,
                TransactionId = command.TransactionId,
                OrderId = command.OrderId,
                Amount = command.Amount
            }
        };

        var expectedPromotionSummary = promotionSummary;
        expectedPromotionSummary.TotalAmountRedeemed -= rewardDifference;
        expectedPromotionSummary.NumberOfTimesRedeemed -= 1;
        expectedPromotionSummary.TotalNumberOfCustomers -= 1;

        var expectedServiceBusMessage = PromotionsEngineBalanceUpdateMessage.Refunded(rewardAmount, ledger.OrderId,
            command.CustomerId);

        await _service.PerformOrderRefundedReconciliationAsync(request, CancellationToken.None);

        A.CallTo(() =>
            _fakePromotionsRepository.UpdatePromotionAsync(A<Promotion>.That.IsEqualTo(expectedPromotion),
                A<CancellationToken>._)).MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakePromotionSummaryRepository.UpdatePromotionSummaryAsync(A<PromotionSummary>.That.IsEqualTo(expectedPromotionSummary), A<CancellationToken>._)).MustHaveHappenedOnceExactly();

        A.CallTo(() =>
                _fakeCustomerOrderRewardsLedgerRepository.UpdateCustomerOrderRewardsLedger(
                    A<CustomerOrderRewardsLedger>.That.Matches(x => RewardsLedgersMatch(x, expectedLedger)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() =>
                _fakeServiceBusManager.SendMessageToServiceBus(
                    A<PromotionsEngineBalanceUpdateMessage>.That.Matches(x => ServiceBusMessagesMatch(x, expectedServiceBusMessage)),
                    _serviceBusOptions.Value.PromotionsEngineBalanceUpdateQueueName, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(RewardsDistributionReconciliationService))]
    [Trait("Method", nameof(RewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync))]
    [Description(
        $"Test the happy path for {nameof(RewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync)}")]
    public async Task Test_Order_Settled_Reconciliation_Success()
    {
        var command = _fixture.Create<OrderSettledCommand>();
        var ledger = _fixture.Build<CustomerOrderRewardsLedger>().Without(x => x.RewardTransactions).Create();

        var expectedLedger = ledger;
        expectedLedger.RewardTransactions = new List<RewardTransaction>
        {
            RewardTransaction.Settled(command.OrderId, 0)
        };

        var expectedServiceBusMessage = PromotionsEngineBalanceUpdateMessage.Settled(0, ledger.OrderId, ledger.CustomerId);

        var request = new ReconcileOrderSettledRequest
        {
            Command = command,
            Ledger = ledger,
            ShouldSettle = true
        };

        await _service.PerformOrderSettledReconciliationAsync(request, CancellationToken.None);

        A.CallTo(() =>
                _fakeCustomerOrderRewardsLedgerRepository.UpdateCustomerOrderRewardsLedger(
                    A<CustomerOrderRewardsLedger>.That.Matches(x => RewardsLedgersMatch(x, expectedLedger)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() =>
                _fakeServiceBusManager.SendMessageToServiceBus(
                    A<PromotionsEngineBalanceUpdateMessage>.That.Matches(x => ServiceBusMessagesMatch(x, expectedServiceBusMessage)),
                    _serviceBusOptions.Value.PromotionsEngineBalanceUpdateQueueName, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(RewardsDistributionReconciliationService))]
    [Trait("Method", nameof(RewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync))]
    [Description(
        $"Test reward negation for {nameof(RewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync)}")]
    public async Task Test_Order_Settled_Reconciliation_Negate_Reward()
    {
        var command = _fixture.Create<OrderSettledCommand>();
        var ledger = _fixture.Build<CustomerOrderRewardsLedger>().Without(x => x.RewardTransactions).Create();
        var promotionSummary = _fixture.Create<PromotionSummary>();
        var promotion = _fixture.Create<Promotion>();

        var expectedLedger = ledger;
        expectedLedger.RewardBalance = 0;
        expectedLedger.RewardTransactions = new List<RewardTransaction>
        {
            RewardTransaction.Settled(command.OrderId, 0)
        };

        var expectedServiceBusMessage = PromotionsEngineBalanceUpdateMessage.Settled(-ledger.RewardBalance, ledger.OrderId, ledger.CustomerId);

        var promotionSummaryGetCall = A.CallTo(() =>
            _fakePromotionSummaryRepository.GetPromotionSummaryAsync(A<string>._, A<CancellationToken>._));
        promotionSummaryGetCall.Returns(promotionSummary);

        var expectedPromotionSummary = promotionSummary;
        expectedPromotionSummary.TotalAmountRedeemed -= ledger.RewardBalance;
        expectedPromotionSummary.NumberOfTimesRedeemed -= 1;
        expectedPromotionSummary.TotalNumberOfCustomers -= 1;

        var promotionGetCall = A.CallTo(() =>
            _fakePromotionsRepository.GetPromotionByIdAsync(A<string>._, A<CancellationToken>._));
        promotionGetCall.Returns(promotion);

        var expectedPromotion = promotion;
        expectedPromotion.CustomerIds.Remove(ledger.CustomerId);

        var request = new ReconcileOrderSettledRequest
        {
            Command = command,
            Ledger = ledger,
            ShouldSettle = false
        };

        await _service.PerformOrderSettledReconciliationAsync(request, CancellationToken.None);

        A.CallTo(() =>
                _fakeCustomerOrderRewardsLedgerRepository.UpdateCustomerOrderRewardsLedger(
                    A<CustomerOrderRewardsLedger>.That.Matches(x => RewardsLedgersMatch(x, expectedLedger)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() =>
                _fakeServiceBusManager.SendMessageToServiceBus(
                    A<PromotionsEngineBalanceUpdateMessage>.That.Matches(x => ServiceBusMessagesMatch(x, expectedServiceBusMessage)),
                    _serviceBusOptions.Value.PromotionsEngineBalanceUpdateQueueName, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        promotionSummaryGetCall.MustHaveHappenedOnceExactly();
        promotionGetCall.MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakePromotionSummaryRepository.UpdatePromotionSummaryAsync(A<PromotionSummary>.That.IsEqualTo(expectedPromotionSummary), A<CancellationToken>._)).MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakePromotionsRepository.UpdatePromotionAsync(A<Promotion>.That.IsEqualTo(expectedPromotion), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static bool RewardsLedgersMatch(CustomerOrderRewardsLedger ledger, CustomerOrderRewardsLedger expected)
    {
        ledger.OrderId.ShouldBe(expected.OrderId);
        ledger.CustomerId.ShouldBe(expected.CustomerId);
        ledger.Merchant.Should().Be(expected.Merchant);
        ledger.Promotion.Should().Be(expected.Promotion);
        ledger.RewardTransactions.Should().BeEquivalentTo(expected.RewardTransactions);

        return true;
    }

    private static bool ServiceBusMessagesMatch(PromotionsEngineBalanceUpdateMessage message,
        PromotionsEngineBalanceUpdateMessage expected)
    {
        message.RewardAmount.ShouldBe(expected.RewardAmount);
        message.OrderId.ShouldBe(expected.OrderId);
        message.CustomerId.ShouldBe(expected.CustomerId);
        message.TransactionType.ShouldBe(expected.TransactionType);

        return true;
    }
}