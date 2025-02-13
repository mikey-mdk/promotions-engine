namespace PromotionsEngine.ServiceBusWorker.Configuration;

public class ServiceBusOptions
{
    public const string ServiceBusSectionName = "PromotionsEngineServiceBus";

    public const string ServiceBusClientName = "ServiceBusClient";

    public string PromotionsEngineTransactionQueueName { get; set; } = string.Empty;
}