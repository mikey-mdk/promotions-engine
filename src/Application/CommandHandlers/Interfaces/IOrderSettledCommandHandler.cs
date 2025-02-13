using PromotionsEngine.Application.Commands;

namespace PromotionsEngine.Application.CommandHandlers.Interfaces;

public interface IOrderSettledCommandHandler
{
    /// <summary>
    /// Handler for the <see cref="OrderSettledCommand"/>.
    /// </summary>
    Task HandleOrderSettledCommand(OrderSettledCommand command, CancellationToken cancellationToken);
}