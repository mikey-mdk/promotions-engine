using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.Validation.Interfaces;

public interface IMerchantValidationEngine
{
    /// <summary>
    /// Validates the properties of the CreateMerchantRequest object.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    List<string> Validate(CreateMerchantRequest request);

    /// <summary>
    /// Validates the properties of the UpdateMerchantRequest object.
    /// Not all these properties are required so only validates the properties provided.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    List<string> Validate(UpdateMerchantRequest request);

    /// <summary>
    /// Validates the properties of the PatchMerchantRequest object.
    /// Not all the properties are required so only validates the properties provided.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    List<string> Validate(PatchMerchantRequest request);
}
