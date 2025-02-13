using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.CommandHandlers.Implementations;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Application.Requests.Reconciliation;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Application.CommandHandlers;

[ExcludeFromCodeCoverage]
public class OrderSettledCommandHandlerTests
{
    private readonly ICustomerOrderRewardsLedgerRepository _fakeCustomerOrderRewardsLedgerRepo;
    private readonly IRewardsDistributionReconciliationService _fakeRewardsDistributionReconciliationService;

    private readonly OrderSettledCommandHandler _orderSettledCommandHandler;

    public OrderSettledCommandHandlerTests()
    {
        _fakeCustomerOrderRewardsLedgerRepo = A.Fake<ICustomerOrderRewardsLedgerRepository>();
        _fakeRewardsDistributionReconciliationService = A.Fake<IRewardsDistributionReconciliationService>();

        _orderSettledCommandHandler = new OrderSettledCommandHandler(
            _fakeCustomerOrderRewardsLedgerRepo,
            _fakeRewardsDistributionReconciliationService);
    }

    [Fact]
    public async Task Should_Send_OrderSettled_With_No_Amount_Change()
    {
        // Arrange
        var rewardBalance = 100m;

        var promotion = new Promotion
        {
            Id = Guid.NewGuid().ToString(),
        };

        var ledger = new CustomerOrderRewardsLedger
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerId = Guid.NewGuid().ToString(),
            RewardBalance = rewardBalance,
            Promotion = promotion,
            Merchant = new Merchant { MerchantName = "merchant-name" }
        };

        var command = new OrderSettledCommand
        {
            OrderId = ledger.OrderId,
            TransactionDetails = new[]
            {
                new MerchantTransactionDetail
                {
                    MerchantName = ledger.Merchant.MerchantName,
                    TotalGatewayCaptured = 1,
                    TotalGatewayRefunded = 0
                }
            }
        };

        A.CallTo(() => _fakeCustomerOrderRewardsLedgerRepo.GetLedgerForOrder(command.OrderId, A<CancellationToken>._)).Returns(ledger);

        var expectedRequest = new ReconcileOrderSettledRequest
        {
            ShouldSettle = true,
            Command = command,
            Ledger = ledger
        };

        // Act
        await _orderSettledCommandHandler.HandleOrderSettledCommand(command, default);

        // Assert
        A.CallTo(() => _fakeRewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync(
                A<ReconcileOrderSettledRequest>.That.Matches(x => RequestObjectsMatch(x, expectedRequest)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Should_Send_OrderSettled_With_Negated_Balance()
    {
        // Arrange
        var rewardBalance = 100m;

        var customerId = Guid.NewGuid().ToString();

        var promotion = new Promotion
        {
            Id = Guid.NewGuid().ToString(),
            CustomerIds = new List<string> { customerId }
        };

        var ledger = new CustomerOrderRewardsLedger
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerId = customerId,
            RewardBalance = rewardBalance,
            Promotion = promotion,
            Merchant = new Merchant { MerchantName = "merchant-name" }
        };

        var command = new OrderSettledCommand
        {
            OrderId = ledger.OrderId,
            TransactionDetails = new[]
            {
                new MerchantTransactionDetail
                {
                    MerchantName = ledger.Merchant.MerchantName,
                    TotalGatewayCaptured = 1,
                    TotalGatewayRefunded = 0
                },
                new MerchantTransactionDetail
                {
                    MerchantName = "disqualified",
                    TotalGatewayCaptured = 1,
                    TotalGatewayRefunded = 0
                }
            }
        };

        A.CallTo(() => _fakeCustomerOrderRewardsLedgerRepo.GetLedgerForOrder(command.OrderId, A<CancellationToken>._)).Returns(ledger);

        var expectedRequest = new ReconcileOrderSettledRequest
        {
            ShouldSettle = false,
            Command = command,
            Ledger = ledger
        };

        // Act
        await _orderSettledCommandHandler.HandleOrderSettledCommand(command, default);

        A.CallTo(() => _fakeRewardsDistributionReconciliationService.PerformOrderSettledReconciliationAsync(
                A<ReconcileOrderSettledRequest>.That.Matches(x => RequestObjectsMatch(x, expectedRequest)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    private bool RequestObjectsMatch(ReconcileOrderSettledRequest actual, ReconcileOrderSettledRequest expected)
    {
        expected.ShouldBeEquivalentTo(actual);

        return true;
    }
}
