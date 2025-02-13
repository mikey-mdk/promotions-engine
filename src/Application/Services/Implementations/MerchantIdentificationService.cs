using Microsoft.Extensions.Logging;
using PromotionsEngine.Application.Cache;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Engines.Interfaces;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.Services.Implementations;

public class MerchantIdentificationService : IMerchantIdentificationService
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IMerchantRegexRepository _merchantRegexRepository;
    private readonly IRegexEvaluationEngine _regexEvaluationEngine;
    private readonly ILogger<MerchantIdentificationService> _logger;

    public MerchantIdentificationService(
        IMerchantRepository merchantRepository,
        IRedisCacheManager redisCacheManager,
        IMerchantRegexRepository merchantRegexRepository,
        IRegexEvaluationEngine regexEvaluationEngine,
        ILogger<MerchantIdentificationService> logger)
    {
        _merchantRepository = merchantRepository;
        _redisCacheManager = redisCacheManager;
        _merchantRegexRepository = merchantRegexRepository;
        _regexEvaluationEngine = regexEvaluationEngine;
        _logger = logger;
    }

    public async Task<Merchant?> IdentifyMerchantByRegexAsync(string merchantName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(merchantName))
        {
            return null;
        }

        try
        {
            //Run merchant regex validation here on the provided MerchantName
            var merchantRegexList = await _redisCacheManager.GetOrSetAsync(CRedisCacheKeys.MerchantRegexLookupCacheKey,
                _merchantRegexRepository.GetAllMerchantRegexItemsAsync);

            if (merchantRegexList is { Count: 0 })
            {
                _logger.LogError("Unable to identify merchant {merchantName} when merchant regex list is null or empty", merchantName);
                return null;
            }

            foreach (var merchantRegex in merchantRegexList!)
            {
                var matches = _regexEvaluationEngine.EvaluateRegexList(merchantName, merchantRegex.RegexPatterns);

                if (matches.Count > 0)
                {
                    return await _merchantRepository.GetMerchantByIdAsync(merchantRegex.Id, cancellationToken);
                }
            }

            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered when attempting to identify merchant {merchantName}", merchantName);
            return null;
        }
    }
}