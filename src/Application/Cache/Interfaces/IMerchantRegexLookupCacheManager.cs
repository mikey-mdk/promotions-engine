namespace PromotionsEngine.Application.Cache.Interfaces;

public interface IMerchantRegexLookupCacheManager
{
    /// <summary>
    /// This method is used to pull the merchant regex lookup data from the database and hydrate the cache.
    /// </summary>
    /// <returns></returns>
    Task HydrateMerchantRegexLookupCache();
}