using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;

namespace PromotionsEngine.Infrastructure.Repositories.Implementations;

public class PromotionSummaryRepository : IPromotionSummaryRepository
{
    private readonly Container _promotionsSummaryContainer;
    private readonly ILogger<PromotionSummaryRepository> _logger;

    public PromotionSummaryRepository(
        IAzureClientFactory<CosmosClient> clientFactory,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        ILogger<PromotionSummaryRepository> logger)
    {
        var client = clientFactory.CreateClient(CosmosDbOptions.CosmosClientName);
        _promotionsSummaryContainer = client.GetDatabase(cosmosDbOptions.Value.DatabaseName)
            .GetContainer(cosmosDbOptions.Value.PromotionSummaryContainerName);
        _logger = logger;
    }

    public async Task<PromotionSummary?> GetPromotionSummaryAsync(string promotionId, CancellationToken cancellationToken)
    {
        try
        {
            var promotionSummaryEntity =
                await _promotionsSummaryContainer.ReadItemAsync<PromotionSummaryEntity>(promotionId,
                    new PartitionKey(promotionId), cancellationToken: cancellationToken);
            return promotionSummaryEntity?.Resource.MapToDomain();
        }
        catch (CosmosException ce) when (ce.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "{exceptionName} encountered attempting to get promotion summary for promotionId: {promotionId}",
                nameof(e), promotionId);
            return null;
        }
    }

    public async Task<PromotionSummary?> UpdatePromotionSummaryAsync(PromotionSummary promotionSummary, CancellationToken cancellationToken)
    {
        try
        {
            var entity = promotionSummary.MapToEntity();

            var updatedResponse = await _promotionsSummaryContainer.UpsertItemAsync(entity, cancellationToken: cancellationToken);

            return updatedResponse.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "{exceptionName} encountered attempting to update a promotion summary for promotionId: {promotionId}",
                nameof(e), promotionSummary.Id);
            return null;
        }
    }
}