namespace PromotionsEngine.Application.Managers.Interfaces;

public interface IServiceBusManager
{
    /// <summary>
    /// This method serializes the provided message and puts it on the service bus queue.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="message"></param>
    /// <param name="queueName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendMessageToServiceBus<TMessage>(TMessage message, string queueName, CancellationToken cancellationToken);
}