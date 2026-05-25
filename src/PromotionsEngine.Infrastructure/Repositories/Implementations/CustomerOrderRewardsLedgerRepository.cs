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

public class CustomerOrderRewardsLedgerRepository : ICustomerOrderRewardsLedgerRepository
{
    private readonly Container _customerOrderRewardsLedgerContainer;
    private readonly ILogger<CustomerOrderRewardsLedgerRepository> _logger;

    public CustomerOrderRewardsLedgerRepository(
        IAzureClientFactory<CosmosClient> clientFactory,
        IOptions<CosmosDbOptions> cosmosDbOptions,
        ILogger<CustomerOrderRewardsLedgerRepository> logger)
    {
        _customerOrderRewardsLedgerContainer = clientFactory.CreateClient(CosmosDbOptions.CosmosClientName)
            .GetDatabase(cosmosDbOptions.Value.DatabaseName)
            .GetContainer(cosmosDbOptions.Value.CustomerOrderRewardsLedgerContainerName);
        _logger = logger;
    }

    public async Task<CustomerOrderRewardsLedger?> GetLedgerForOrder(string orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            var queryable = _customerOrderRewardsLedgerContainer
                .GetItemLinqQueryable<CustomerOrderRewardsLedgerEntity>(allowSynchronousQueryExecution: true)
                .Where(x => x.OrderId == orderId);

            using var feed = queryable.ToFeedIterator();

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync(cancellationToken);
                var entity = response.FirstOrDefault();
                if (entity != null)
                {
                    return entity.MapToDomain();
                }
            }

            return new CustomerOrderRewardsLedger();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Exception encountered attempting to get rewards ledger for orderId: {orderId}",
                orderId);
            return null;
        }
    }

    public async Task<List<CustomerOrderRewardsLedger>> GetLedgersForCustomer(string customerId, CancellationToken cancellationToken)
    {
        try
        {
            var queryable = _customerOrderRewardsLedgerContainer
                .GetItemLinqQueryable<CustomerOrderRewardsLedgerEntity>(allowSynchronousQueryExecution: true)
                .Where(x => x.CustomerId == customerId);

            using var feed = queryable.ToFeedIterator();

            var ledgers = new List<CustomerOrderRewardsLedger>();
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync(cancellationToken);
                ledgers.AddRange(response.Select(e => e.MapToDomain()));
            }

            return ledgers;
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Exception encountered attempting to get ledgers for customerId: {customerId}",
                customerId);
            return new List<CustomerOrderRewardsLedger>();
        }
    }

    public async Task<CustomerOrderRewardsLedger?> CreateCustomerOrderRewardsLedger(CustomerOrderRewardsLedger ledger, CancellationToken cancellationToken)
    {
        try
        {
            var entity = ledger.MapToEntity();

            var created =
                await _customerOrderRewardsLedgerContainer.CreateItemAsync(entity, new PartitionKey(entity.CustomerId), cancellationToken: cancellationToken);

            return created.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "Exception encountered attempting to create ledger");
            return null;
        }
    }

    public async Task<CustomerOrderRewardsLedger> UpdateCustomerOrderRewardsLedger(CustomerOrderRewardsLedger ledger, CancellationToken cancellationToken)
    {
        try
        {
            var entity = ledger.MapToEntity();

            var updated = await _customerOrderRewardsLedgerContainer.UpsertItemAsync(entity,
                new PartitionKey(entity.CustomerId), cancellationToken: cancellationToken);

            return updated.Resource.MapToDomain();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to update ledger for orderId: {orderId}", ledger.OrderId);
            return ledger;
        }
    }
}