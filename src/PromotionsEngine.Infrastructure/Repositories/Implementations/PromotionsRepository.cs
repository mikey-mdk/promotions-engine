using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Requests;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;
using PromotionsEngine.Infrastructure.Extensions;

namespace PromotionsEngine.Infrastructure.Repositories.Implementations;

[ExcludeFromCodeCoverage(Justification = "Infrastructure")]
public class PromotionsRepository : IPromotionsRepository
{
    private readonly Container _promotionsContainer;
    private readonly ILogger<PromotionsRepository> _logger;

    public PromotionsRepository(
        IAzureClientFactory<CosmosClient> clientFactory,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        ILogger<PromotionsRepository> logger)
    {
        var client = clientFactory.CreateClient(CosmosDbOptions.CosmosClientName);
        _promotionsContainer = client
            .GetDatabase(cosmosDbOptions.Value.DatabaseName)
            .GetContainer(cosmosDbOptions.Value.PromotionsContainerName);
        _logger = logger;
    }

    public async Task<Promotion> GetPromotionByIdAsync(string promotionId, CancellationToken cancellationToken)
    {
        var entityResponse = await GetPromotionEntity(promotionId, cancellationToken);
        return entityResponse == null ? new Promotion() : entityResponse.MapToDomain();
    }

    public async Task<List<Promotion>> GetPromotionsByMerchantIdAsync(string merchantId, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = _promotionsContainer.GetItemLinqQueryable<PromotionEntity>(allowSynchronousQueryExecution: true);

            var matches = queryable
                .Where(p => p.Active)
                .Where(p => p.MerchantId == merchantId);

            using var linqFeed = matches.ToFeedIterator();

            var promotions = new List<Promotion>();
            while (linqFeed.HasMoreResults)
            {
                var response = await linqFeed.ReadNextAsync(cancellationToken);

                promotions.AddRange(response.Select(p => p.MapToDomain()));
            }

            return promotions;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered while attempting to get promotions by merchantId: {merchantId}",
                merchantId);
            return new List<Promotion>();
        }
    }

    public async Task<List<Promotion>> GetPromotionsFromQueryAsync(GetPromotionsQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = _promotionsContainer.GetItemLinqQueryable<PromotionEntity>(allowSynchronousQueryExecution: true);

            var matches = queryable
                .Where(p => p.Active == queryRequest.Active)
                .AddFilterIfValueExists(queryRequest.MerchantId, p => p.MerchantId == queryRequest.MerchantId)
                .AddFilterIfValueExists(queryRequest.StartDate, p => p.PromotionStartDate >= queryRequest.StartDate!.Value.Date)
                .AddFilterIfValueExists(queryRequest.EndDate, p => p.PromotionEndDate <= queryRequest.EndDate!.Value.Date);

            using var linqFeed = matches.ToFeedIterator();

            var promotions = new List<Promotion>();
            while (linqFeed.HasMoreResults)
            {
                var response = await linqFeed.ReadNextAsync(cancellationToken);

                promotions.AddRange(response.Select(p => p.MapToDomain()));
            }

            return promotions;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered while attempting to query Promotions container");
            return new List<Promotion>();
        }
    }

    public async Task<Promotion> CreatePromotionAsync(Promotion promotion, CancellationToken cancellationToken)
    {
        var entity = promotion.MapToEntity();

        var createResponse = await _promotionsContainer.CreateItemAsync(entity, new PartitionKey(promotion.Id), cancellationToken: cancellationToken);

        return createResponse == null ? new Promotion() : createResponse.Resource.MapToDomain();
    }

    public async Task<Promotion> UpdatePromotionAsync(Promotion promotion, CancellationToken cancellationToken)
    {
        var entity = promotion.MapToEntity();

        var updateResponse = await _promotionsContainer.UpsertItemAsync(entity, new PartitionKey(promotion.Id), cancellationToken: cancellationToken);

        return updateResponse == null ? new Promotion() : updateResponse.Resource.MapToDomain();
    }

    public async Task<Promotion> DeletePromotionAsync(string promotionId, CancellationToken cancellationToken)
    {
        var entity = await GetPromotionEntity(promotionId, cancellationToken);
        if (entity == null)
        {
            return new Promotion();
        }

        entity.Deleted = true;

        // Do we even care about the response when deleting?

        var updateResponse = await _promotionsContainer.UpsertItemAsync(entity, new PartitionKey(promotionId), cancellationToken: cancellationToken);

        return updateResponse == null ? new Promotion() : updateResponse.Resource.MapToDomain();
    }

    private async Task<PromotionEntity?> GetPromotionEntity(string promotionId, CancellationToken cancellationToken)
    {
        try
        {
            return await _promotionsContainer.ReadItemAsync<PromotionEntity>(promotionId, new PartitionKey(promotionId),
                cancellationToken: cancellationToken);
        }
        catch (CosmosException ce) when (ce.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "{exceptionName} encountered attempting to get promotion {promotionId}", nameof(e), promotionId);
            return null;
        }
    }
}
