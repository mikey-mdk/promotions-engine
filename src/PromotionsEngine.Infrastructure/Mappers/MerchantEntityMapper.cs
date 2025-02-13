using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.Mappers;

public static class MerchantEntityMapper
{
    public static MerchantEntity MapToEntity(this Merchant merchant)
    {
        return new MerchantEntity
        {
            Id = merchant.Id,
            MerchantId = merchant.MerchantId,
            ExternalMerchantId = merchant.ExternalMerchantId,
            MerchantName = merchant.MerchantName,
            Description = merchant.Description,
            MerchantAddress = MapToEntity(merchant.MerchantAddress),
            MerchantType = merchant.MerchantType,
            BusinessType = merchant.BusinessType,
            RegexPatterns = merchant.RegexPatterns,
            CreatedDateTime = merchant.CreatedDateTime,
            ModifiedDateTime = merchant.ModifiedDateTime,
            Deleted = merchant.Deleted,
            Active = merchant.Active,
            SchemaVersion = merchant.SchemaVersion,
        };
    }

    public static Merchant MapToDomain(this MerchantEntity merchantEntity)
    {
        return new Merchant
        {
            Id = merchantEntity.Id,
            MerchantId = merchantEntity.MerchantId,
            ExternalMerchantId = merchantEntity.ExternalMerchantId,
            MerchantName = merchantEntity.MerchantName,
            Description = merchantEntity.Description,
            MerchantAddress = MapToDomain(merchantEntity.MerchantAddress),
            MerchantType = merchantEntity.MerchantType,
            BusinessType = merchantEntity.BusinessType,
            RegexPatterns = merchantEntity.RegexPatterns,
            CreatedDateTime = merchantEntity.CreatedDateTime,
            ModifiedDateTime = merchantEntity.ModifiedDateTime,
            Deleted = merchantEntity.Deleted,
            Active = merchantEntity.Active,
            SchemaVersion = merchantEntity.SchemaVersion
        };
    }

    private static MerchantAddressEntity MapToEntity(MerchantAddress address)
    {
        return new MerchantAddressEntity
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country
        };
    }

    private static MerchantAddress MapToDomain(MerchantAddressEntity address)
    {
        return new MerchantAddress
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country
        };
    }
}