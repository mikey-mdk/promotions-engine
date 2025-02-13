using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.BusMessageHandlers.Implementations;
using PromotionsEngine.Application.BusMessages;
using PromotionsEngine.Application.BusMessages.PurchaseLedger;
using PromotionsEngine.Application.CommandHandlers.Interfaces;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Domain.Enumerations;

namespace PromotionsEngine.Tests.Application.BusMessageHandlers;

[ExcludeFromCodeCoverage]
public class PromotionsEngineTransactionMessageHandlerTests
{
    private readonly IOrderCreatedCommandHandler _fakeOrderCreatedCommandHandler;
    private readonly IOrderRefundedCommandHandler _fakeOrderRefundedCommandHandler;
    private readonly IOrderSettledCommandHandler _fakeOrderSettledCommandHandler;

    private readonly PromotionsEngineTransactionMessageHandler _transactionMessageHandler;

    public PromotionsEngineTransactionMessageHandlerTests()
    {
        _fakeOrderCreatedCommandHandler = A.Fake<IOrderCreatedCommandHandler>();
        _fakeOrderRefundedCommandHandler = A.Fake<IOrderRefundedCommandHandler>();
        _fakeOrderSettledCommandHandler = A.Fake<IOrderSettledCommandHandler>();

        _transactionMessageHandler = new PromotionsEngineTransactionMessageHandler(
            _fakeOrderCreatedCommandHandler,
            _fakeOrderRefundedCommandHandler,
            _fakeOrderSettledCommandHandler);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PromotionsEngineTransactionMessage_OrderCreated")]
    [Description("Test message is processed and sent to the correct handler")]
    public async Task Test_Handle_Order_Created_Message_Type()
    {
        var orderId = Guid.NewGuid().ToString();

        var message = new PromotionsEngineTransactionMessage
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            TransactionType = TransactionTypeEnum.OrderCreated.Name
        };

        var commandConfig = A.CallTo(() =>
            _fakeOrderCreatedCommandHandler.HandleOrderCreatedCommand(
                A<OrderCreatedCommand>.That.Matches(x => x.OrderId == orderId), default));

        await _transactionMessageHandler.Handle(message);

        commandConfig.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PromotionsEngineTransactionMessage_OrderRefunded")]
    [Description("Test message is processed and sent to the correct handler")]
    public async Task Test_Handle_Order_Refunded_Message_Type()
    {
        var orderId = Guid.NewGuid().ToString();

        var message = new PromotionsEngineTransactionMessage
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            TransactionType = TransactionTypeEnum.OrderRefunded.Name
        };

        var commandConfig = A.CallTo(() =>
            _fakeOrderRefundedCommandHandler.HandleOrderRefundedCommand(
                A<OrderRefundedCommand>.That.Matches(x => x.OrderId == orderId), default));

        await _transactionMessageHandler.Handle(message);

        commandConfig.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PromotionsEngineTransactionMessage_OrderSettled")]
    [Description("Test message is processed and sent to the correct handler")]
    public async Task Test_Handle_Order_Settled_Message_Type()
    {
        var orderId = Guid.NewGuid().ToString();

        var message = new PromotionsEngineTransactionMessage
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            TransactionType = TransactionTypeEnum.OrderSettled.Name,
            TransactionDetails = new List<PromotionsEngineTransactionMessage.MerchantTransactionDetail>
            {
                new ()
                {
                    MerchantName = "merchant-name",
                    TotalGatewayCaptured = 2,
                    TotalGatewayRefunded = 1
                }
            }
        };

        var commandConfig = A.CallTo(() => _fakeOrderSettledCommandHandler.HandleOrderSettledCommand(
            A<OrderSettledCommand>.That.Matches(x =>
                x.OrderId == orderId &&
                x.TransactionDetails.Any(t => t.MerchantName == "merchant-name" && t.TotalGatewayCaptured == 2 && t.TotalGatewayRefunded == 1))
            , default));

        await _transactionMessageHandler.Handle(message);

        commandConfig.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PromotionsEngineTransactionMessage")]
    [Description("Test exception is thrown when message type is unknown")]
    public async Task Test_Handle_Unknown_Message_Type()
    {
        var message = new PromotionsEngineTransactionMessage
        {

            TransactionType = TransactionTypeEnum.Unknown.Name
        };

        await Should.ThrowAsync<ArgumentException>(async () =>
            await _transactionMessageHandler.Handle(message));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PurchaseLedgerOrderCreated")]
    [Description("Should map event to command and call approprtiate handler")]
    public async Task Should_Handle_PurchaseLedgerOrderCreated()
    {
        // Arrange
        var message = new PurchaseLedgerOrderCreated { OrderId = Guid.NewGuid() };

        var call = A.CallTo(() =>
            _fakeOrderCreatedCommandHandler.HandleOrderCreatedCommand(
                A<OrderCreatedCommand>.That.Matches(x => x.OrderId == message.OrderId.ToString()), default));

        // Act
        await _transactionMessageHandler.Handle(message);

        // Assert
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PurchaseLedgerOrderRefunded")]
    [Description("Should map event to command and call approprtiate handler")]
    public async Task Should_Handle_PurchaseLedgerOrderRefunded()
    {
        // Arrange
        var message = new PurchaseLedgerOrderRefunded { OrderId = Guid.NewGuid() };

        var call = A.CallTo(() =>
            _fakeOrderRefundedCommandHandler.HandleOrderRefundedCommand(
                A<OrderRefundedCommand>.That.Matches(x => x.OrderId == message.OrderId.ToString()), default));

        // Act
        await _transactionMessageHandler.Handle(message);

        // Assert
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", nameof(PromotionsEngineTransactionMessageHandler))]
    [Trait("Method", "Handle_PurchaseLedgerSettled")]
    [Description("Should map event to command and call approprtiate handler")]
    public async Task Should_Handle_PurchaseLedgerSettled()
    {
        // Arrange
        var message = new PurchaseLedgerSettled
        {
            PurchaseId = Guid.NewGuid(),
            TransactionDetails = new List<PurchaseLedgerSettled.MerchantTransactionDetail>
            {
                new ()
                {
                    MerchantName = "merchant-name",
                    TotalGatewayCaptured = 2,
                    TotalGatewayRefunded = 1,
                }
            }
        };

        var call = A.CallTo(() => _fakeOrderSettledCommandHandler.HandleOrderSettledCommand(
            A<OrderSettledCommand>.That.Matches(x =>
                x.OrderId == message.PurchaseId.ToString() &&
                x.TransactionDetails.Any(t => t.MerchantName == "merchant-name" && t.TotalGatewayCaptured == 2 && t.TotalGatewayRefunded == 1)),
            default));

        // Act
        await _transactionMessageHandler.Handle(message);

        // Assert
        call.MustHaveHappenedOnceExactly();
    }
}