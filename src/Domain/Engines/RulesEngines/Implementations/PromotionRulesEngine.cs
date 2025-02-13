using Microsoft.Extensions.Logging;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Helpers.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Domain.Engines.RulesEngines.Implementations;

public class PromotionRulesEngine : IPromotionRulesEngine
{
    private readonly IPromotionSummaryRepository _promotionSummaryRepository;
    private readonly IPromotionRulesEngineHelper _promotionRulesEngineHelper;
    private readonly ILogger<PromotionRulesEngine> _logger;

    public PromotionRulesEngine(
        IPromotionSummaryRepository promotionSummaryRepository,
        IPromotionRulesEngineHelper promotionRulesEngineHelper,
        ILogger<PromotionRulesEngine> logger)
    {
        _promotionSummaryRepository = promotionSummaryRepository;
        _promotionRulesEngineHelper = promotionRulesEngineHelper;
        _logger = logger;
    }

    public async Task<List<(Promotion promotion, PromotionSummary promotionSummary)>> FindValidPromotions(FindValidPromotionsRequest request)
    {
        var validPromotions = new List<(Promotion, PromotionSummary)>();
        foreach (var promotion in request.Promotions)
        {
            if (!await EvaluateDefaultPromotionRules(promotion, request.OrderAmount, request.EvaluationContext, request.CustomerId))
            {
                continue;
            }

            var promotionSummary =
                await _promotionSummaryRepository.GetPromotionSummaryAsync(promotion.Id, CancellationToken.None);

            if (promotionSummary == null)
            {
                //If we don't find a promotion summary for the promotion we assume its not valid and don't add it to the response.
                _logger.LogWarning("Promotion Summary was null during rules evaluation for Promotion {promotionId}", promotion.Id);
                continue;
            }

            var isValid = await EvaluatePromotionSummaryRules(promotion, promotionSummary, request.OrderAmount, request.EvaluationContext);

            if (isValid)
            {
                validPromotions.Add(new ValueTuple<Promotion, PromotionSummary>(promotion, promotionSummary));
            }
        }

        return validPromotions;
    }

    public async Task<bool> EvaluateDefaultPromotionRules(Promotion promotion, decimal orderAmount, string evaluationContext, string? customerId = null)
    {
        switch (evaluationContext)
        {
            case CPromotionRuleEvaluationContext.OrderCreated:
            case CPromotionRuleEvaluationContext.CheckoutPresentation:
            case CPromotionRuleEvaluationContext.AppPresentation:
                return await _promotionRulesEngineHelper.EvaluateAllDefaultRules(promotion, orderAmount, customerId);
            case CPromotionRuleEvaluationContext.OrderRefunded:
                return await _promotionRulesEngineHelper.EvaluateDefaultRulesForRefundContext(promotion, orderAmount);
            default:
                _logger.LogError("Invalid PromotionRulesEvaluationContext Encountered: {evaluationContext}",
                    evaluationContext);
                return false;
        }
    }

    public async Task<bool> EvaluatePromotionSummaryRules(Promotion promotion, PromotionSummary promotionSummary, decimal orderAmount, string evaluationContext)
    {
        switch (evaluationContext)
        {
            //We want to allow refunds no matter what the state of the promotion summary is.
            //This may also be true for CheckoutPresentation context. TBD.
            case CPromotionRuleEvaluationContext.OrderRefunded:
                return true;
            case CPromotionRuleEvaluationContext.OrderCreated:
            case CPromotionRuleEvaluationContext.CheckoutPresentation:
            case CPromotionRuleEvaluationContext.AppPresentation:
                return await _promotionRulesEngineHelper.EvaluateAllSummaryRules(promotion, promotionSummary);
            default:
                _logger.LogError("Invalid PromotionRulesEvaluationContext Encountered: {evaluationContext}", evaluationContext);
                return false;
        }
    }
}