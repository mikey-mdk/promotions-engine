using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using System.Text.Json.Serialization;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using PromotionsEngine.ServiceBusWorker.Configuration;

namespace PromotionsEngine.ServiceBusWorker.Extensions;

public static class AzureClientExtensions
{
    public static async Task AddAzureClientConnections(this HostApplicationBuilder builder)
    {
        var cosmosDbOptions = builder.Configuration.GetSection(CosmosDbOptions.CosmosDbOptionsSectionName).Get<CosmosDbOptions>()!;
        builder.Services.Configure<CosmosDbOptions>(options => builder.Configuration.GetSection(CosmosDbOptions.CosmosDbOptionsSectionName).Bind(options));

        var cosmosClient = await new CosmosClientBuilder(builder.Configuration.GetConnectionString("CosmosSqlPromotionsEngine"))
            .WithThrottlingRetryOptions(TimeSpan.FromSeconds(1), 5)
            .WithCustomSerializer(new CosmosSystemTextJsonSerializer(new System.Text.Json.JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
            }))
            .BuildAndInitializeAsync(new List<(string database, string container)>
            {
                new(cosmosDbOptions.DatabaseName, cosmosDbOptions.MerchantContainerName),
                new(cosmosDbOptions.DatabaseName, cosmosDbOptions.PromotionsContainerName)
            });

        builder.Services.AddAzureClients(clientsBuilder =>
        {
            clientsBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"))
                .WithName(ServiceBusOptions.ServiceBusClientName)
                .ConfigureOptions(options =>
                {
                    options.TransportType = ServiceBusTransportType.AmqpWebSockets;

                    // RetryOptions do not apply to message processing. Default message delivery count is 10.
                    options.RetryOptions.Delay = TimeSpan.FromMilliseconds(50);
                    options.RetryOptions.MaxDelay = TimeSpan.FromSeconds(5);
                    options.RetryOptions.MaxRetries = 3;
                });

            clientsBuilder.AddClient<CosmosClient, CosmosClientOptions>((_, _, _) => cosmosClient)
                .WithName(CosmosDbOptions.CosmosClientName);
        });
    }
}