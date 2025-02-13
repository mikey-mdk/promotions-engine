using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Repositories.Interfaces;

public interface ICustomerOrderRewardsLedgerRepository
{
    /// <summary>
    /// /Get the rewards ledger for the provided orderId.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CustomerOrderRewardsLedger?> GetLedgerForOrder(string orderId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all the rewards ledgers for the provided customerId.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<CustomerOrderRewardsLedger>> GetLedgersForCustomer(string customerId, CancellationToken cancellationToken);

    /// <summary>
    /// Create the CustomerOrderRewardsLedger document.
    /// </summary>
    /// <param name="ledger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CustomerOrderRewardsLedger?> CreateCustomerOrderRewardsLedger(CustomerOrderRewardsLedger ledger, CancellationToken cancellationToken);

    /// <summary>
    /// Update the CustomerOrderRewardsLedger document.
    /// </summary>
    /// <param name="ledger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CustomerOrderRewardsLedger> UpdateCustomerOrderRewardsLedger(CustomerOrderRewardsLedger ledger, CancellationToken cancellationToken);
}