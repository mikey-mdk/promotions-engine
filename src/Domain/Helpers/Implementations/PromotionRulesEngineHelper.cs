using PromotionsEngine.Domain.Helpers.Interfaces;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Helpers.Implementations;

public class PromotionRulesEngineHelper : IPromotionRulesEngineHelper
{
    public async Task<bool> EvaluateAllDefaultRules(Promotion promotion, decimal orderAmount, string? customerId = null)
    {
        if (!promotion.Active)
        {
            return false;
        }

        if (promotion.PromotionStartDate.Date > DateTime.UtcNow.Date || promotion.PromotionEndDate.Date < DateTime.UtcNow.Date)
        {
            return false;
        }

        if (promotion.PromotionRules.MinimumTransactionAmount > orderAmount)
        {
            return false;
        }

        //Not every promotion will define a max transaction amount.
        if (promotion.PromotionRules.MaximumTransactionAmount.HasValue)
        {
            return promotion.PromotionRules.MaximumTransactionAmount >= orderAmount;
        }

        //Not every workflow will have a customer id.
        if (!string.IsNullOrEmpty(customerId))
        {
            var redemptions = promotion.CustomerIds.Count(x => x == customerId);
            return redemptions < promotion.PromotionRules.NumberOfRedemptionsPerCustomer;
        }

        return true;
    }

    public async Task<bool> EvaluateDefaultRulesForRefundContext(Promotion promotion, decimal orderAmount)
    {
        return promotion.PromotionRules.MinimumTransactionAmount <= orderAmount;
    }

    public async Task<bool> EvaluateAllSummaryRules(Promotion promotion, PromotionSummary promotionSummary)
    {
        if (promotion.PromotionRules.NumberOfTimesRedeemable.HasValue 
            && promotion.PromotionRules.NumberOfTimesRedeemable <= promotionSummary.NumberOfTimesRedeemed)
        {
            return false;
        }

        if (promotion.PromotionRules.TotalRewardsAmount.HasValue 
            && promotion.PromotionRules.TotalRewardsAmount <= promotionSummary.TotalAmountRedeemed)
        {
            return false;
        }

        return !promotion.PromotionRules.TotalNumberOfCustomers.HasValue
               || !(promotion.PromotionRules.TotalNumberOfCustomers <= promotionSummary.TotalNumberOfCustomers);
    }
}