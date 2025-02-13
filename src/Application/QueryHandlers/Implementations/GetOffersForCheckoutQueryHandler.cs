using Microsoft.Extensions.Logging;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Application.Mappers;
using PromotionsEngine.Application.Queries;
using PromotionsEngine.Application.QueryHandlers.Interfaces;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Application.QueryHandlers.Implementations;

public class GetOffersForCheckoutQueryHandler : IGetOffersForCheckoutQueryHandler
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IPromotionsRepository _promotionsRepository;
    private readonly IPromotionRulesEngine _promotionRulesEngine;
    private readonly IRewardsCalculationEngine _rewardsCalculationEngine;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly ILogger<GetOffersForCheckoutQueryHandler> _logger;

    public GetOffersForCheckoutQueryHandler(
        IMerchantRepository merchantRepository, 
        IPromotionsRepository promotionsRepository, 
        IPromotionRulesEngine promotionRulesEngine, 
        IRewardsCalculationEngine rewardsCalculationEngine, 
        IRedisCacheManager redisCacheManager, 
        ILogger<GetOffersForCheckoutQueryHandler> logger)
    {
        _merchantRepository = merchantRepository;
        _promotionsRepository = promotionsRepository;
        _promotionRulesEngine = promotionRulesEngine;
        _rewardsCalculationEngine = rewardsCalculationEngine;
        _redisCacheManager = redisCacheManager;
        _logger = logger;
    }

    public async Task<GetCheckoutOffersDto> GetOffersForCheckoutAsync(GetOffersForCheckoutQuery query, CancellationToken cancellationToken)
    {
        var (merchant, promotions) = await _redisCacheManager.GetOrSetAsync(
            query.MerchantId, async () =>
            {
                var merchant = await _merchantRepository.GetMerchantByIdAsync(query.MerchantId, cancellationToken);

                if (merchant == null)
                {
                    _logger.LogError("Unable to find merchant for GetOffersForCheckoutWorkflow with merchantId: {merchantId}", query.MerchantId);
                    return (new Merchant(), new List<Promotion>());
                }

                var promotions =
                    await _promotionsRepository.GetPromotionsByMerchantIdAsync(merchant.MerchantId, cancellationToken);

                return (merchant, promotions);
            });

        if (promotions.Count == 0)
        {
            return new GetCheckoutOffersDto();
        }

        //We are assuming here Checkout wants to evaluate promotion summary rules for offers. If not, we can change this to call EvaluateDefaultPromotionRules.
        var promotionsByPromotionSummary = await _promotionRulesEngine.FindValidPromotions(new FindValidPromotionsRequest
        {
            Promotions = promotions,
            OrderAmount = query.OrderAmount,
            EvaluationContext = CPromotionRuleEvaluationContext.CheckoutPresentation
        });

        if (promotionsByPromotionSummary.Count == 0)
        {
            return new GetCheckoutOffersDto();
        }

        var checkoutOfferDtoList = await Task.WhenAll(promotionsByPromotionSummary.Select(async x =>
        {
            var reward = await _rewardsCalculationEngine.CalculateRewardsAsync(new RewardsCalculationRequest
            {
                OrderAmount = query.OrderAmount,
                RateAmount = x.promotion.RateAmount,
                RewardRateType = x.promotion.RewardRateTypeEnum!
            });
            return CheckoutOfferDtoMapper.MapToDto(merchant, x.promotion, query.OrderAmount, reward);
        }));
        
        return new GetCheckoutOffersDto
        {
            CheckoutOffers = checkoutOfferDtoList.ToList()
        };
    }
}