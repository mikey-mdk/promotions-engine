using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PromotionsEngine.Application.BusMessageHandlers.Interfaces;
using PromotionsEngine.Application.BusMessages;
using PromotionsEngine.Application.BusMessages.PurchaseLedger;
using PromotionsEngine.ServiceBusWorker.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.ServiceBusWorker;

[ExcludeFromCodeCoverage]
public class ServiceBusWorker : BackgroundService
{
    private readonly ILogger<ServiceBusWorker> _logger;
    private readonly ServiceBusOptions _serviceBusOptions;
    private readonly IPromotionsEngineTransactionMessageHandler _PromotionsEngineTransactionMessageHandler;

    private readonly ServiceBusClient _serviceBusClient;

    public ServiceBusWorker(
        ILogger<ServiceBusWorker> logger,
        IOptions<ServiceBusOptions> serviceBusOptions,
        IPromotionsEngineTransactionMessageHandler PromotionsEngineTransactionMessageHandler,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory)
    {
        _logger = logger;
        _PromotionsEngineTransactionMessageHandler = PromotionsEngineTransactionMessageHandler;
        _serviceBusOptions = serviceBusOptions.Value;

        _serviceBusClient = serviceBusClientFactory.CreateClient(ServiceBusOptions.ServiceBusClientName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        try
        {
            // the processor that reads and processes messages from the queue
            var serviceBusProcessor = _serviceBusClient.CreateSessionProcessor(
                _serviceBusOptions.PromotionsEngineTransactionQueueName,
                new ServiceBusSessionProcessorOptions());

            // add handler to process messages
            serviceBusProcessor.ProcessMessageAsync += ProcessServiceBusMessage;

            // add handler to process any errors
            serviceBusProcessor.ProcessErrorAsync += ServiceBusErrorHandler;

            // start processing 
            await serviceBusProcessor.StartProcessingAsync(stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception Encountered when attempting to initialize service bus queue processor");
        }
    }

    private async Task ProcessServiceBusMessage(ProcessSessionMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        Console.WriteLine($"Service Bus Message Received: {args.Message.Subject}, {body}");

        switch (args.Message.Subject)
        {
            case nameof(PurchaseLedgerOrderCreated):
                var purchaseLedgerOrderCreated = Deserialize<PurchaseLedgerOrderCreated>(body);
                await _PromotionsEngineTransactionMessageHandler.Handle(purchaseLedgerOrderCreated);
                break;
            case nameof(PurchaseLedgerOrderRefunded):
                var purchaseLedgerOrderRefunded = Deserialize<PurchaseLedgerOrderRefunded>(body);
                await _PromotionsEngineTransactionMessageHandler.Handle(purchaseLedgerOrderRefunded);
                break;
            case nameof(PurchaseLedgerSettled):
                var purchaseLedgerSettled = Deserialize<PurchaseLedgerSettled>(body);
                await _PromotionsEngineTransactionMessageHandler.Handle(purchaseLedgerSettled);
                break;
            default:
                var transactionMessage = Deserialize<PromotionsEngineTransactionMessage>(body);
                await _PromotionsEngineTransactionMessageHandler.Handle(transactionMessage);
                break;
        }

        // Complete the message. message is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }

    private static T Deserialize<T>(string body)
    {
        return JsonConvert.DeserializeObject<T>(body)
            ?? throw new JsonSerializationException("Message null after deserialization");
    }

    private Task ServiceBusErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Failed to process message from service bus.");
        return Task.CompletedTask;
    }
}