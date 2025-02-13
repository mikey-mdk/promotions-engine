using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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

public class MerchantRegexRepository : IMerchantRegexRepository
{
    private readonly Container _merchantRegexContainer;
    private readonly ILogger<MerchantRegexRepository> _logger;

    public MerchantRegexRepository(
        IAzureClientFactory<CosmosClient> clientFactory,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        ILogger<MerchantRegexRepository> logger)
    {
        var client = clientFactory.CreateClient(CosmosDbOptions.CosmosClientName);
        _merchantRegexContainer = client
            .GetDatabase(cosmosDbOptions.Value.DatabaseName)
            .GetContainer(cosmosDbOptions.Value.MerchantRegexLookupContainerName);
        _logger = logger;
    }

    public async Task<List<MerchantRegex>> GetAllMerchantRegexItemsAsync()
    {
        try
        {
            //The number of documents in this container should never be more than the number of merchants in the system.
            //Adding a Take(1000) which should far exceed the number of merchants in the system but provide some protection against a runaway query.
            using var iterator = _merchantRegexContainer
                .GetItemLinqQueryable<MerchantRegexEntity>()
                .Take(1000)
                .ToFeedIterator();

            var merchantRegexList = new List<MerchantRegex>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                merchantRegexList.AddRange(response.Select(x => x.MapToDomain()));
            }

            return merchantRegexList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to retrieve all MerchantRegex items");
            return new List<MerchantRegex>();
        }
    }

    public async Task<MerchantRegex?> CreateMerchantRegexAsync(MerchantRegex merchantRegex, CancellationToken cancellationToken)
    {
        try
        {
            var merchantRegexEntity = merchantRegex.MapToEntity();

            var response = await _merchantRegexContainer.CreateItemAsync(merchantRegexEntity,
                new PartitionKey(merchantRegexEntity.Id), cancellationToken: cancellationToken);
            return response.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to create MerchantRegex with id: {merchantId}", merchantRegex.Id);
            return null;
        }
    }

    public async Task<MerchantRegex?> ReplaceRegexPatternsAsync(MerchantRegex merchantRegex, CancellationToken cancellationToken)
    {
        try
        {
            var patchOptions = new PatchItemRequestOptions()
            {
                FilterPredicate = $"FROM c WHERE c.id = '{merchantRegex.Id}'"
            };

            var patchOperations = new List<PatchOperation>()
            {
                PatchOperation.Replace($"/{nameof(MerchantRegex.RegexPatterns)}", merchantRegex.RegexPatterns)
            };

            var response = await _merchantRegexContainer.PatchItemAsync<MerchantRegexEntity>(
                id: merchantRegex.Id,
                partitionKey: new PartitionKey(merchantRegex.Id),
                patchOperations: patchOperations,
                requestOptions: patchOptions,
                cancellationToken);

            return response.Resource?.MapToDomain();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("MerchantRegex document with Id {id} not found. Creating document now", merchantRegex.Id);
            return await CreateMerchantRegexAsync(merchantRegex, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to append entry to {containerName}", nameof(CosmosDbOptions.MerchantRegexLookupContainerName));
            return null;
        }
    }
}