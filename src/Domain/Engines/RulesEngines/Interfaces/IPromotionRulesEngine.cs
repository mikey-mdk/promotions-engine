using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;

public interface IPromotionRulesEngine
{
    /// <summary>
    /// Returns a list of promotions by promotion summary tuples that are valid for the given order context.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<List<(Promotion promotion, PromotionSummary promotionSummary)>> FindValidPromotions(FindValidPromotionsRequest request);

    /// <summary>
    /// This method will evaluate the top level promotion rules that don't depend on the promotion summary.
    /// If any rule evaluates to false the entire promotion is considered invalid.
    /// </summary>
    /// <param name="promotion"></param>
    /// <param name="orderAmount"></param>
    /// <param name="evaluationContext">see <see cref="CPromotionRuleEvaluationContext"/></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public Task<bool> EvaluateDefaultPromotionRules(Promotion promotion, decimal orderAmount, string evaluationContext, string? customerId = null);

    /// <summary>
    /// This method will evaluate the PromotionSummary rules which are aggregated from the promotion's transactions.
    /// If any rule evaluates to false the entire promotion is considered invalid.
    /// If the promotions is considered valid we return a tuple with a bool and the retrieved promotion summary.
    /// This allows the caller to update the promotion summary without needing to make a duplicate db call.
    /// </summary>
    /// <param name="promotion"></param>
    /// <param name="promotionSummary"></param>
    /// <param name="orderAmount"></param>
    /// <param name="evaluationContext">see <see cref="CPromotionRuleEvaluationContext"/></param>
    /// <returns></returns>
    public Task<bool> EvaluatePromotionSummaryRules(Promotion promotion, PromotionSummary promotionSummary, decimal orderAmount, string evaluationContext);
}