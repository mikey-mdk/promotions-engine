using PromotionsEngine.Application.BusMessages;
using PromotionsEngine.Application.BusMessages.PurchaseLedger;

namespace PromotionsEngine.Application.BusMessageHandlers.Interfaces;

public interface IPromotionsEngineTransactionMessageHandler
{
    /// <summary>
    /// Handles the incoming service bus message and distributes it to the correct command handler.
    /// </summary>
    Task Handle(PromotionsEngineTransactionMessage message);

    /// <summary>
    /// Handle a PurchaseLedgerOrderCreated event (via bus message).
    /// </summary>
    Task Handle(PurchaseLedgerOrderCreated message);

    /// <summary>
    /// Handle a PurchaseLedgerOrderRefunded event (via bus message).
    /// </summary>
    Task Handle(PurchaseLedgerOrderRefunded message);

    /// <summary>
    /// Handle a PurchaseLedgerSettled event (via bus message).
    /// </summary>
    Task Handle(PurchaseLedgerSettled message);
}