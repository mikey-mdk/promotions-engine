using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using PromotionsEngine.Application.Configuration;
using PromotionsEngine.Application.Managers.Interfaces;

namespace PromotionsEngine.Application.Managers.Implementations;

public class ServiceBusManager : IServiceBusManager
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<ServiceBusManager> _logger;

    public ServiceBusManager(
        IAzureClientFactory<ServiceBusClient> clientFactory,
        ILogger<ServiceBusManager> logger)
    {
        _client = clientFactory.CreateClient(ServiceBusOptions.ServiceBusClientName);
        _logger = logger;
    }

    public async Task SendMessageToServiceBus<TMessage>(TMessage message, string queueName, CancellationToken cancellationToken)
    {
        try
        {
            var sender = _client.CreateSender(queueName);

            var jsonMessage = JsonSerializer.Serialize(message);

            var serviceBusMessage = new ServiceBusMessage(jsonMessage);

            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Encountered exception attempting to put message ${message} on the service bus queue {queueName}",
                message, queueName);
        }
    }
}