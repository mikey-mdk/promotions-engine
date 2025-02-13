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

namespace PromotionsEngine.Infrastructure.Repositories.Implementations;

public class MerchantRepository : IMerchantRepository
{
    private readonly Container _merchantContainer;
    private readonly ILogger<MerchantRepository> _logger;

    public MerchantRepository(
        IAzureClientFactory<CosmosClient> clientFactory,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        ILogger<MerchantRepository> logger)
    {
        var client = clientFactory.CreateClient(CosmosDbOptions.CosmosClientName);
        _merchantContainer = client
                                    .GetDatabase(cosmosDbOptions.Value.DatabaseName)
                                    .GetContainer(cosmosDbOptions.Value.MerchantContainerName);
        _logger = logger;
    }

    public async Task<Merchant?> GetMerchantByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var merchantEntity = await _merchantContainer.ReadItemAsync<MerchantEntity>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return merchantEntity?.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to get merchant by id: {merchantId}", id);
            return null;
        }
    }

    public async Task<List<Merchant>> GetMerchantsByQueryAsync(GetMerchantsQueryRequest request, CancellationToken cancellationToken)
    {
        if (request.MerchantIds.Count == 0)
        {
            return new List<Merchant>();
        }

        try
        {
            var queryable = _merchantContainer.GetItemLinqQueryable<MerchantEntity>(allowSynchronousQueryExecution: true);

            var matches = queryable
                .Where(m => m.Active == request.Active)
                .Where(m => request.MerchantIds.Contains(m.MerchantId));

            using var linqFeed = matches.ToFeedIterator();

            var merchants = new List<Merchant>();
            while (linqFeed.HasMoreResults)
            {
                var response = await linqFeed.ReadNextAsync(cancellationToken);

                merchants.AddRange(response.Select(m => m.MapToDomain()));
            }

            return merchants;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered while attempting to query merchant container");
            return new List<Merchant>();
        }
    }

    public async Task<Merchant?> CreateMerchantAsync(Merchant merchant, CancellationToken cancellationToken)
    {
        try
        {
            var entity = merchant.MapToEntity();

            var createResponse = await _merchantContainer.CreateItemAsync(entity, new PartitionKey(merchant.Id), cancellationToken: cancellationToken);
            return createResponse.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to create merchant with id: {merchantId}", merchant.Id);
            return null;
        }
    }

    public async Task<Merchant?> UpdateMerchantAsync(Merchant merchant, CancellationToken cancellationToken)
    {
        try
        {
            var entity = merchant.MapToEntity();

            var updateResponse = await _merchantContainer.UpsertItemAsync(entity, new PartitionKey(merchant.Id), cancellationToken: cancellationToken);

            return updateResponse.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to update merchant with id: {merchantId}", merchant.Id);
            return null;
        }
    }

    public async Task<Merchant?> PatchMerchantAsync(PatchMerchantRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MerchantAddress != null)
            {
                //We attempt to patch the address first. If that operation fails we will not proceed with the rest of the patch.
                var addressPatchResponse = await PatchMerchantAddress(request.MerchantAddress, request.Id, cancellationToken);
                if (addressPatchResponse == null)
                {
                    return null;
                }
            }

            var operations = new List<PatchOperation>();

            if (!string.IsNullOrEmpty(request.ExternalMerchantId))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.ExternalMerchantId)}",
                    request.ExternalMerchantId));
            }

            if (!string.IsNullOrEmpty(request.MerchantName))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantName)}",
                    request.MerchantName));
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.Description)}",
                    request.Description));
            }

            if (!string.IsNullOrEmpty(request.MerchantType))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantType)}",
                    request.MerchantType));
            }

            if (!string.IsNullOrEmpty(request.BusinessType))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.BusinessType)}",
                    request.BusinessType));
            }

            if (request.Active != null)
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.Active)}",
                    request.Active));
            }

            if (request.RegexPatterns is { Count: > 0 })
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.RegexPatterns)}",
                                       request.RegexPatterns));
            }

            if (operations.Count == 0 && request.MerchantAddress == null)
            {
                return null;
            }

            var response = await _merchantContainer.PatchItemAsync<MerchantEntity>(
                id: request.Id,
                partitionKey: new PartitionKey(request.Id),
                patchOperations: operations,
                cancellationToken: cancellationToken);

            return response.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to patch the Merchant document");
            return null;
        }
    }

    public async Task<Merchant?> DeleteMerchantAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var delete = await _merchantContainer.PatchItemAsync<MerchantEntity>(
            id: id,
            partitionKey: new PartitionKey(id),
            patchOperations: new[] { PatchOperation.Set($"/{nameof(MerchantEntity.Deleted)}", true) },
            cancellationToken: cancellationToken);

            return delete.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to delete merchant with id: {merchantId}", id);
            return null;
        }
    }

    private async Task<Merchant?> PatchMerchantAddress(
        PatchMerchantAddressRequest request,
        string merchantId,
        CancellationToken cancellationToken)
    {
        try
        {
            var operations = new List<PatchOperation>();

            if (!string.IsNullOrEmpty(request.AddressLine1))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.AddressLine1)}",
                                           request.AddressLine1));
            }

            if (!string.IsNullOrEmpty(request.AddressLine2))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.AddressLine2)}",
                                           request.AddressLine2));
            }

            if (!string.IsNullOrEmpty(request.City))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.City)}",
                                           request.City));
            }

            if (!string.IsNullOrEmpty(request.State))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.State)}",
                                           request.State));
            }

            if (!string.IsNullOrEmpty(request.ZipCode))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.ZipCode)}",
                                           request.ZipCode));
            }

            if (!string.IsNullOrEmpty(request.Country))
            {
                operations.Add(PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.Country)}",
                                           request.Country));
            }

            var response = await _merchantContainer.PatchItemAsync<MerchantEntity>(
                id: merchantId,
                partitionKey: new PartitionKey(merchantId),
                patchOperations: operations,
                cancellationToken: cancellationToken);

            return response.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to patch the Merchant Address document");
            return null;
        }
    }
}