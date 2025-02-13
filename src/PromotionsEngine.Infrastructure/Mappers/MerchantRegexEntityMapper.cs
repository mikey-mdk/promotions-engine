using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.Mappers;

public static class MerchantRegexEntityMapper
{
    public static MerchantRegex MapToDomain(this MerchantRegexEntity entity)
    {
        return new MerchantRegex
        {
            Id = entity.Id,
            RegexPatterns = entity.RegexPatterns
        };
    }

    public static MerchantRegexEntity MapToEntity(this MerchantRegex domain)
    {
        return new MerchantRegexEntity
        {
            Id = domain.Id,
            RegexPatterns = domain.RegexPatterns
        };
    }
}