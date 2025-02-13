using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.Configuration;

[ExcludeFromCodeCoverage]
public class ServiceBusOptions
{
    public static readonly string ServiceBusSectionName = "PromotionsEngineServiceBus";

    public static readonly string ServiceBusClientName = "ServiceBusClient";

    public string PromotionsEngineTransactionQueueName { get; set; } = string.Empty;

    public string PromotionsEngineBalanceUpdateQueueName { get; set; } = string.Empty;
}