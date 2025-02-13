using PromotionsEngine.Application.Dtos.Merchant;
using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.Services.Interfaces;

/// <summary>
/// This is the CRUD service for the Merchant domain model.
/// This service is will communicate with the MerchantRepository and will return MerchantDto objects.
/// The reason this is an Application layer service and not a domain service is because this service takes in non domain request arguments and is responsible for
/// mapping the Merchant domain object to the MerchantDto object which is not a domain concern.
/// </summary>
public interface IMerchantService
{
    /// <summary>
    /// This method is used to find the Merchant by the provided id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantDto> GetMerchantByIdAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to create the Merchant document.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantDto> CreateMerchantAsync(CreateMerchantRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to update the Merchant document.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantDto> UpdateMerchantAsync(UpdateMerchantRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to patch individual fields of the Merchant document.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantDto> PatchMerchantAsync(PatchMerchantRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// This method is used to mark the Merchant document as deleted.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MerchantDto> DeleteMerchantAsync(string id, CancellationToken cancellationToken);
}