using PromotionsEngine.Application.Dtos.Merchant;
using PromotionsEngine.Application.Exceptions;
using PromotionsEngine.Application.Mappers;
using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.Services.Implementations;

public class MerchantService : IMerchantService
{
    private readonly IMerchantRepository _merchantRepository;

    public MerchantService(
        IMerchantRepository merchantRepository)
    {
        _merchantRepository = merchantRepository;
    }

    public async Task<MerchantDto> GetMerchantByIdAsync(string id, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetMerchantByIdAsync(id, cancellationToken);
        return merchant?.MapToMerchantDto() ?? new MerchantDto();
    }

    public async Task<MerchantDto> CreateMerchantAsync(CreateMerchantRequest request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid().ToString();
        var newMerchant = new Merchant
        {
            Id = id,
            CreatedDateTime = DateTime.UtcNow,
            ModifiedDateTime = DateTime.UtcNow,
            MerchantId = id,
            ExternalMerchantId = request.ExternalMerchantId,
            MerchantName = request.MerchantName,
            Description = request.Description,
            MerchantType = request.MerchantType,
            BusinessType = request.BusinessType,
            MerchantAddress = request.MerchantAddress.MapToMerchantAddressDomain(),
            Active = request.Active,
        };

        var createdMerchant = await _merchantRepository.CreateMerchantAsync(newMerchant, cancellationToken);

        if (createdMerchant == null)
        {
            throw new DomainObjectNullException($"Failed to create merchant with Id {id}");
        }

        return createdMerchant.MapToMerchantDto();
    }

    public async Task<MerchantDto> UpdateMerchantAsync(UpdateMerchantRequest request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetMerchantByIdAsync(request.Id, cancellationToken)
                       ?? throw new DomainObjectNullException(
                           $"Failed to update merchant with Id {request.Id} because the merchant does not exist");

        // Update from the request:
        if (!string.IsNullOrEmpty(request.ExternalMerchantId))
        {
            merchant!.ExternalMerchantId = request.ExternalMerchantId;
        }

        if (!string.IsNullOrEmpty(request.MerchantName))
        {
            merchant.MerchantName = request.MerchantName;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            merchant.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.MerchantType))
        {
            merchant.MerchantType = request.MerchantType;
        }

        if (!string.IsNullOrEmpty(request.BusinessType))
        {
            merchant.BusinessType = request.BusinessType;
        }

        if (request.MerchantAddress != null)
        {
            merchant.MerchantAddress = request.MerchantAddress.MapToMerchantAddressDomain();
        }

        if (request.RegexPatterns is { Count: > 0 })
        {
            merchant.RegexPatterns = request.RegexPatterns;
        }

        if (request.Active != null)
        {
            merchant.Active = request.Active;
        }

        // Update Metadata:
        merchant.ModifiedDateTime = DateTime.UtcNow;

        var updatedMerchant = await _merchantRepository.UpdateMerchantAsync(merchant, cancellationToken);

        return updatedMerchant?.MapToMerchantDto() ??
               throw new DomainObjectNullException($"Failed to update merchant with Id {request.Id}");
    }

    public async Task<MerchantDto> PatchMerchantAsync(PatchMerchantRequest request, CancellationToken cancellationToken)
    {
        var patchedMerchant = await _merchantRepository.PatchMerchantAsync(request, cancellationToken);

        return patchedMerchant?.MapToMerchantDto() ??
               throw new DomainObjectNullException($"Failed to patch merchant with Id {request.Id}");
    }

    public async Task<MerchantDto> DeleteMerchantAsync(string id, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.DeleteMerchantAsync(id, cancellationToken);
        return merchant?.MapToMerchantDto() ?? throw new DomainObjectNullException($"Failed to delete merchant with Id {id}");
    }
}