using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Infrastructure.ChangeFeed.Interfaces;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.ChangeFeed.Implementations;

[ExcludeFromCodeCoverage(Justification = "Cosmos Change feed library currently prevents proper unit testing patterns")]
public class CosmosChangeFeedProcessor : ICosmosChangeFeedProcessor
{
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosDbOptions _cosmosDbOptions;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IMerchantRegexRepository _merchantRegexRepository;
    private readonly ILogger<CosmosChangeFeedProcessor> _logger;

    public CosmosChangeFeedProcessor(
        IAzureClientFactory<CosmosClient> clientFactory,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        IRedisCacheManager redisCacheManager,
        ILogger<CosmosChangeFeedProcessor> logger,
        IMerchantRegexRepository merchantRegexRepository)
    {
        _cosmosClient = clientFactory.CreateClient(CosmosDbOptions.CosmosClientName);
        _cosmosDbOptions = cosmosDbOptions.Value;
        _redisCacheManager = redisCacheManager;
        _logger = logger;
        _merchantRegexRepository = merchantRegexRepository;
    }

    public async Task SetupCosmosChangeFeedProcessors()
    {
        try
        {
            var merchantLeaseContainer =
                _cosmosClient.GetContainer(_cosmosDbOptions.DatabaseName, _cosmosDbOptions.MerchantLeaseContainerName);
            var promotionsLeaseContainer =
                _cosmosClient.GetContainer(_cosmosDbOptions.DatabaseName, _cosmosDbOptions.PromotionsLeaseContainerName);

            var merchantChangeFeedProcessor = _cosmosClient
                .GetContainer(_cosmosDbOptions.DatabaseName, _cosmosDbOptions.MerchantContainerName)
                .GetChangeFeedProcessorBuilder<MerchantEntity>(processorName: "merchantChangeFeedProcessor",
                    onChangesDelegate: HandleMerchantChangesAsync)
                .WithInstanceName("WebApplicationHost")
                .WithLeaseContainer(merchantLeaseContainer)
                .Build();

            var promotionsChangeFeedProcessor = _cosmosClient
                .GetContainer(_cosmosDbOptions.DatabaseName, _cosmosDbOptions.PromotionsContainerName)
                .GetChangeFeedProcessorBuilder<PromotionEntity>(processorName: "promotionChangeFeedProcessor",
                    onChangesDelegate: HandlePromotionChangesAsync)
                .WithInstanceName("WebApplicationHost")
                .WithLeaseContainer(promotionsLeaseContainer)
                .Build();

            await merchantChangeFeedProcessor.StartAsync();
            _logger.LogInformation("Change feed processor {processorName} started", nameof(merchantChangeFeedProcessor));

            await promotionsChangeFeedProcessor.StartAsync();
            _logger.LogInformation("Change feed processor {processorName} started", nameof(promotionsChangeFeedProcessor));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception Encountered attempting to initialize cosmos change feed processor");
        }
    }

    private async Task HandleMerchantChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<MerchantEntity> changes,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (var item in changes)
            {
                _logger.LogInformation("Detected operation for item with id {merchantId}, created at {createdDateTime}.", item.MerchantId, item.CreatedDateTime);

                await _redisCacheManager.InvalidateCache(item.MerchantId);
                await _merchantRegexRepository.ReplaceRegexPatternsAsync(
                    new MerchantRegex
                    {
                        Id = item.MerchantId,
                        RegexPatterns = item.RegexPatterns

                    }, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to process change feed for merchant");
        }
    }

    private async Task HandlePromotionChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<PromotionEntity> changes,
        CancellationToken cancellationToken)
    {
        try
        {
            foreach (var item in changes)
            {
                _logger.LogInformation("Detected operation for item with id {id}, created at {createdDateTime}.", item.Id,
                    item.CreatedDateTime);

                await _redisCacheManager.InvalidateCache(item.Id);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to process change feed for Promotion");
        }
    }
}