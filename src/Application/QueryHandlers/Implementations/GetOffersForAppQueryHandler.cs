using PromotionsEngine.Application.Cache;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Application.Queries;
using PromotionsEngine.Application.QueryHandlers.Interfaces;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.QueryHandlers.Implementations;

public class GetOffersForAppQueryHandler : IGetOffersForAppQueryHandler
{
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IPromotionsRepository _promotionsRepository;
    private readonly IMerchantRepository _merchantRepository;

    public GetOffersForAppQueryHandler(
        IRedisCacheManager redisCacheManager, 
        IPromotionsRepository promotionsRepository, 
        IMerchantRepository merchantRepository)
    {
        _redisCacheManager = redisCacheManager;
        _promotionsRepository = promotionsRepository;
        _merchantRepository = merchantRepository;
    }

    public async Task<GetAppOffersDto> GetOffersForAppAsync(GetOffersForAppQuery query, CancellationToken cancellationToken)
    {
        var (merchants, promotions) = await _redisCacheManager.GetOrSetAsync(CRedisCacheKeys.AppOffersCacheKey,
            async () =>
            {
                var promotions = await _promotionsRepository.GetPromotionsFromQueryAsync(new GetPromotionsQueryRequest
                {
                    Active = true

                }, cancellationToken);

                if (promotions.Count == 0)
                {
                    return (new List<Merchant>(), new List<Promotion>());
                }

                var merchantIds = promotions.Select(x => x.MerchantId).Distinct().ToList();

                var merchants = await _merchantRepository.GetMerchantsByQueryAsync(new GetMerchantsQueryRequest
                {
                    Active = true,
                    MerchantIds = merchantIds
                }, cancellationToken);

                return (merchants, promotions);
            });

        if (promotions.Count == 0)
        {
            return new GetAppOffersDto();
        }

        var response = new List<AppOfferDto>();
        foreach (var merchant in merchants)
        {
            var promotionsForMerchant = promotions
                .Where(x => x.MerchantId == merchant.MerchantId).ToList();

            if (promotionsForMerchant.Count == 0)
            {
                continue;
            }

            response.AddRange(promotionsForMerchant.Select(promotion => new AppOfferDto
            {
                ExternalMerchantId = merchant.ExternalMerchantId,
                Name = promotion.PromotionName,
                Type = promotion.PromotionTypeEnum?.Name ?? string.Empty,
                StartDate = promotion.PromotionStartDate,
                RateFixed = promotion.RewardRateTypeEnum!.Equals(RewardRateTypeEnum.Fixed)
                    ? promotion.RateAmount
                    : 0,
                RatePercentage = promotion.RewardRateTypeEnum.Equals(RewardRateTypeEnum.Percentage)
                    ? promotion.RateAmount
                    : 0
            }));
        }

        return new GetAppOffersDto
        {
            AppOfferDtos = response,
            HasMore = false,
            Token = string.Empty
        };
    }
}