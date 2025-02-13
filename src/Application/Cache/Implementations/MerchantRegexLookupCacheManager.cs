using Microsoft.Extensions.Logging;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.Cache.Implementations;

public class MerchantRegexLookupCacheManager : IMerchantRegexLookupCacheManager
{
    private readonly IMerchantRegexRepository _merchantRegexRepository;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly ILogger<MerchantRegexLookupCacheManager> _logger;

    public MerchantRegexLookupCacheManager(
        IMerchantRegexRepository merchantRegexRepository,
        IRedisCacheManager redisCacheManager,
        ILogger<MerchantRegexLookupCacheManager> logger)
    {
        _merchantRegexRepository = merchantRegexRepository;
        _redisCacheManager = redisCacheManager;
        _logger = logger;
    }

    public async Task HydrateMerchantRegexLookupCache()
    {
        try
        {
            await _redisCacheManager.GetOrSetAsync(CRedisCacheKeys.MerchantRegexLookupCacheKey,
                _merchantRegexRepository.GetAllMerchantRegexItemsAsync);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to hydrate MerchantRegexLookupCache");
        }
    }
}