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
            var customerOrderRewardsLedgerEntity =
                await _customerOrderRewardsLedgerContainer.ReadItemAsync<CustomerOrderRewardsLedgerEntity>(id: orderId,
                    partitionKey: new PartitionKey(orderId), cancellationToken: cancellationToken);

            return customerOrderRewardsLedgerEntity == null
                ? new CustomerOrderRewardsLedger()
                : CustomerOrderRewardsLedgerMapper.MapToDomain(customerOrderRewardsLedgerEntity);
        }
        catch (CosmosException ce) when (ce.StatusCode == HttpStatusCode.NotFound)
        {
            //Cosmos throws an exception for every not found request and we don't want to write logs for each of these
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

    public Task<List<CustomerOrderRewardsLedger>> GetLedgersForCustomer(string customerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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

    public Task<CustomerOrderRewardsLedger> UpdateCustomerOrderRewardsLedger(CustomerOrderRewardsLedger ledger, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}