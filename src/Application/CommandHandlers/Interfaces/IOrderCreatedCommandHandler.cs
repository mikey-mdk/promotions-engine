using PromotionsEngine.Application.Commands;

namespace PromotionsEngine.Application.CommandHandlers.Interfaces;

public interface IOrderCreatedCommandHandler
{
    /// <summary>
    /// Business logic for handling an order created command.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleOrderCreatedCommand(OrderCreatedCommand command, CancellationToken cancellationToken);
}