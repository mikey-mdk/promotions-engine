using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Mappers;

public static class CheckoutOfferDtoMapper
{
    public static CheckoutOfferDto MapToDto(Merchant merchant, Promotion promotion, decimal orderAmount, decimal? discountAmount)
    {
        return new CheckoutOfferDto
        {
            MerchantId = merchant.MerchantId,
            ExternalMerchantId = merchant.ExternalMerchantId,
            MerchantName = merchant.MerchantName,
            PromotionName = promotion.PromotionName,
            PromotionDescription = promotion.PromotionDescription,
            StartDate = promotion.PromotionStartDate,
            EndDate = promotion.PromotionEndDate,
            OrderAmount = orderAmount,
            DiscountAmount = discountAmount
        };
    }
}