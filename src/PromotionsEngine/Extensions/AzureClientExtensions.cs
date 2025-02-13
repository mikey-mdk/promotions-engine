using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Azure;
using PromotionsEngine.Application.Configuration;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PromotionsEngine.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class AzureClientExtensions
{
    public static async Task AddAzureClientConnections(this WebApplicationBuilder builder)
    {
        // The CosmosClient can be setup with more granular options using the CosmosClientOptions class.
        var cosmosDbOptions = builder.Configuration.GetSection(CosmosDbOptions.CosmosDbOptionsSectionName)
            .Get<CosmosDbOptions>()!;
        var serviceBusOptions = builder.Configuration.GetSection(ServiceBusOptions.ServiceBusSectionName)
            .Get<ServiceBusOptions>();

        // Using BuildAndInitializeAsync here warms up the SDK so that all caches for the dbs and containers are initialized on startup.
        // This works when you can be sure that the Dbs and the Containers are already created in the Azure environment.
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
                new(cosmosDbOptions.DatabaseName, cosmosDbOptions.PromotionsContainerName),
                new(cosmosDbOptions.DatabaseName, cosmosDbOptions.PromotionSummaryContainerName),
                new(cosmosDbOptions.DatabaseName, cosmosDbOptions.MerchantLeaseContainerName),
                new(cosmosDbOptions.DatabaseName, cosmosDbOptions.PromotionsLeaseContainerName),
            });

        builder.Services.AddAzureClients(clientBuilder =>
        {
            //AddCosmosClient extensions is currently in Microsoft Extension teams backlog. https://github.com/Azure/azure-cosmos-dotnet-v3/issues/4002
            clientBuilder.AddClient<CosmosClient, CosmosClientOptions>((_, _, _) => cosmosClient)
                .WithName(CosmosDbOptions.CosmosClientName);
            clientBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"))
                .WithName(ServiceBusOptions.ServiceBusClientName);
        });
    }
}