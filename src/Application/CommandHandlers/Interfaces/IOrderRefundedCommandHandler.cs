using PromotionsEngine.Application.Commands;

namespace PromotionsEngine.Application.CommandHandlers.Interfaces;

public interface IOrderRefundedCommandHandler
{
    /// <summary>
    /// Handler for the <see cref="OrderRefundedCommand"/>.
    /// </summary>
    Task HandleOrderRefundedCommand(OrderRefundedCommand command, CancellationToken cancellationToken);
}