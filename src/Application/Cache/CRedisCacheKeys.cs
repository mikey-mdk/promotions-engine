namespace PromotionsEngine.Application.Cache;

public static class CRedisCacheKeys
{
    /// <summary>
    /// This key is used to store the list of promotions for the get offers for app workflow.
    /// </summary>
    public const string AppOffersCacheKey = "PromotionsForAppOffersWorkflow";

    public const string MerchantRegexLookupCacheKey = "MerchantRegexLookupCacheKey";
}