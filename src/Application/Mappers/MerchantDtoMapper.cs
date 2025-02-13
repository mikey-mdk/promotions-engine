using PromotionsEngine.Application.Dtos.Merchant;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Mappers;

public static class MerchantDtoMapper
{
    public static MerchantDto MapToMerchantDto(this Merchant merchant)
    {
        return new MerchantDto
        {
            Id = merchant.Id,
            MerchantId = merchant.MerchantId,
            ExternalMerchantId = merchant.ExternalMerchantId,
            MerchantName = merchant.MerchantName,
            Description = merchant.Description,
            MerchantType = merchant.MerchantType,
            BusinessType = merchant.BusinessType,
            MerchantAddress = MapToMerchantAddressDto(merchant.MerchantAddress),
            RegexPatterns = merchant.RegexPatterns,
            CreatedDateTime = merchant.CreatedDateTime,
            ModifiedDateTime = merchant.ModifiedDateTime,
            Deleted = merchant.Deleted,
            Active = merchant.Active
        };
    }

    private static MerchantAddressDto MapToMerchantAddressDto(MerchantAddress address)
    {
        return new MerchantAddressDto
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country
        };
    }

    public static MerchantAddress MapToMerchantAddressDomain(this MerchantAddressDto merchantAddressDto)
    {
        return new MerchantAddress
        {
            AddressLine1 = merchantAddressDto.AddressLine1,
            AddressLine2 = merchantAddressDto.AddressLine2,
            City = merchantAddressDto.City,
            State = merchantAddressDto.State,
            ZipCode = merchantAddressDto.ZipCode,
            Country = merchantAddressDto.Country
        };
    }
}