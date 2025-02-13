using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Application.Validation.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.Validation.Implementations;

public class MerchantValidationEngine : IMerchantValidationEngine
{
    public List<string> Validate(CreateMerchantRequest request)
    {
        var validationErrors = new List<string?>
        {
            ValidateRequiredString(nameof(request.MerchantName), request.MerchantName),
            ValidateMerchantType(request.MerchantType),
            ValidateBusinessType(request.BusinessType),
            ValidateRequiredList(nameof(request.RegexPatterns), request.RegexPatterns)
        };

        return validationErrors.Where(x => !string.IsNullOrEmpty(x)).ToList()!;
    }

    /// <summary>
    /// Not all properties are required for Update. But we do want to perform validation if they are provided.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public List<string> Validate(UpdateMerchantRequest request)
    {
        var validationErrors = new List<string?>
        {
            ValidateRequiredString(nameof(request.Id), request.Id)
        };

        if (!string.IsNullOrEmpty(request.BusinessType))
        {
            validationErrors.Add(ValidateBusinessType(request.BusinessType));
        }

        if (!string.IsNullOrEmpty(request.MerchantType))
        {
            validationErrors.Add(ValidateMerchantType(request.MerchantType));
        }

        return validationErrors.Where(x => !string.IsNullOrEmpty(x)).ToList()!;
    }

    public List<string> Validate(PatchMerchantRequest request)
    {
        var validationErrors = new List<string?>
        {
            ValidateRequiredString(nameof(request.Id), request.Id)
        };

        if (!string.IsNullOrEmpty(request.BusinessType))
        {
            validationErrors.Add(ValidateBusinessType(request.BusinessType));
        }

        if (!string.IsNullOrEmpty(request.MerchantType))
        {
            validationErrors.Add(ValidateMerchantType(request.MerchantType));
        }

        return validationErrors.Where(x => !string.IsNullOrEmpty(x)).ToList()!;
    }

    private string? ValidateRequiredString(string propertyName, string value)
    {
        return string.IsNullOrEmpty(value) ? $"{propertyName} is required" : null;
    }

    private string? ValidateRequiredList<T>(string propertyName, List<T> value)
    {
        return value.Count <= 0 ? $"{propertyName} must include at least one element" : null;
    }

    private string? ValidateMerchantType(string merchantType)
    {
        return !CMerchantTypes.GetAll().Contains(merchantType)
            ? "Provided merchant type is invalid"
            : null;
    }

    private string? ValidateBusinessType(string businessType)
    {
        return !CBusinessTypes.GetAll().Contains(businessType)
            ? "Provided business type is invalid" : null;
    }
}