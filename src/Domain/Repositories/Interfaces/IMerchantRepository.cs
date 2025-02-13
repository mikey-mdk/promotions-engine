using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Domain.Repositories.Interfaces;

/// <summary>
/// This repository is responsible for the CRUD operations for the Merchant document
/// in the Merchant container in the PromotionsEngine CosmosDB instance. 
/// </summary>
public interface IMerchantRepository
{
    /// <summary>
    /// This method is used to find a merchant document by the provided Id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Merchant?> GetMerchantByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to query for merchants based on the provided query request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Merchant>> GetMerchantsByQueryAsync(GetMerchantsQueryRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to create the Merchant document in the container.
    /// </summary>
    /// <param name="merchant"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Merchant?> CreateMerchantAsync(Merchant merchant, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to Update the Merchant document in the container.
    /// </summary>
    /// <param name="merchant"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Merchant?> UpdateMerchantAsync(Merchant merchant, CancellationToken cancellationToken);

    /// <summary>
    /// Performs a partial update on the Merchant document in the container.
    /// Only attempts to patch the properties with values.
    /// NOTE: Only 10 operations are allowed per patch request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Merchant?> PatchMerchantAsync(PatchMerchantRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to mark the Merchant document as deleted in the container.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Merchant?> DeleteMerchantAsync(string id, CancellationToken cancellationToken);
}