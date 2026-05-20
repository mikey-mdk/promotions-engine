using PromotionsEngine.Application.BusMessageHandlers.Interfaces;
using PromotionsEngine.Application.BusMessages;
using PromotionsEngine.Application.BusMessages.PurchaseLedger;
using PromotionsEngine.Application.CommandHandlers.Interfaces;
using PromotionsEngine.Application.Commands;
using PromotionsEngine.Domain.Enumerations;

namespace PromotionsEngine.Application.BusMessageHandlers.Implementations;

public class PromotionsEngineTransactionMessageHandler : IPromotionsEngineTransactionMessageHandler
{
    private readonly IOrderCreatedCommandHandler _orderCreatedCommandHandler;
    private readonly IOrderRefundedCommandHandler _orderRefundedCommandHandler;
    private readonly IOrderSettledCommandHandler _orderSettledCommandHandler;

    public PromotionsEngineTransactionMessageHandler(
        IOrderCreatedCommandHandler orderCreatedCommandHandler,
        IOrderRefundedCommandHandler orderRefundedCommandHandler,
        IOrderSettledCommandHandler orderSettledCommandHandler)
    {
        _orderCreatedCommandHandler = orderCreatedCommandHandler;
        _orderRefundedCommandHandler = orderRefundedCommandHandler;
        _orderSettledCommandHandler = orderSettledCommandHandler;
    }

    public async Task Handle(PromotionsEngineTransactionMessage message)
    {
        switch (message.TransactionType)
        {
            case var type when type.Equals(TransactionTypeEnum.OrderCreated.Name):
            {
                var command = new OrderCreatedCommand
                {
                    OrderId = message.OrderId,
                    CustomerId = message.CustomerId,
                    ExternalMerchantId = message.MerchantId,
                    AuthorizationId = message.AuthorizationId,
                    MerchantName = message.MerchantName,
                    OrderAmount = message.Amount,
                };

                await _orderCreatedCommandHandler.HandleOrderCreatedCommand(command, CancellationToken.None);
                break;
            }
            case var type when type.Equals(TransactionTypeEnum.OrderRefunded.Name):
            {
                var command = new OrderRefundedCommand
                {
                    TransactionId = message.TransactionId,
                    OrderId = message.OrderId,
                    CustomerId = message.CustomerId,
                    ExternalMerchantId = message.MerchantId,
                    Amount = message.Amount,
                    CurrencyCode = message.CurrencyCode
                };

                await _orderRefundedCommandHandler.HandleOrderRefundedCommand(command, CancellationToken.None);
                break;
            }
            case var type when type.Equals(TransactionTypeEnum.OrderSettled.Name):
            {
                var command = new OrderSettledCommand
                {
                    OrderId = message.OrderId.ToString(),
                    TransactionDetails = message.TransactionDetails.Select(x => new MerchantTransactionDetail
                    {
                        MerchantName = x.MerchantName,
                        TotalGatewayCaptured = x.TotalGatewayCaptured,
                        TotalGatewayRefunded = x.TotalGatewayRefunded,
                    })
                };

                await _orderSettledCommandHandler.HandleOrderSettledCommand(command, CancellationToken.None);
                break;
            }
            case var type when string.IsNullOrWhiteSpace(type) || type.Equals(TransactionTypeEnum.Unknown.Name):
            {
                throw new ArgumentException("Encountered unknown transaction type. Unable to process message.");
            }
        }
    }

    public async Task Handle(PurchaseLedgerOrderCreated message)
    {
        var command = new OrderCreatedCommand
        {
            OrderId = message.OrderId.ToString(),
            CustomerId = message.CustomerId.ToString(),
            ExternalMerchantId = message.MerchantId.ToString(),
            AuthorizationId = message.AuthorizationId,
            MerchantName = message.AuthorizationMerchantName,
            OrderAmount = message.Amount,
        };

        await _orderCreatedCommandHandler.HandleOrderCreatedCommand(command, CancellationToken.None);
    }

    public async Task Handle(PurchaseLedgerOrderRefunded message)
    {
        var command = new OrderRefundedCommand
        {
            TransactionId = message.TransactionId,
            OrderId = message.OrderId.ToString(),
            CustomerId = message.CustomerId?.ToString() ?? string.Empty,
            ExternalMerchantId = message.MerchantId?.ToString() ?? string.Empty,
            Amount = message.Amount,
            CurrencyCode = message.Currency
        };

        await _orderRefundedCommandHandler.HandleOrderRefundedCommand(command, CancellationToken.None);
    }

    public async Task Handle(PurchaseLedgerSettled message)
    {
        var command = new OrderSettledCommand
        {
            OrderId = message.PurchaseId.ToString(),
            TransactionDetails = message.TransactionDetails?.Select(x => new MerchantTransactionDetail
            {
                MerchantName = x.MerchantName,
                TotalGatewayCaptured = x.TotalGatewayCaptured,
                TotalGatewayRefunded = x.TotalGatewayRefunded,
            }) ?? Enumerable.Empty<MerchantTransactionDetail>()
        };

        await _orderSettledCommandHandler.HandleOrderSettledCommand(command, CancellationToken.None);
    }
}