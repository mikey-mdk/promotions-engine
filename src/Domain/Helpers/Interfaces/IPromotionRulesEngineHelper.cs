using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Helpers.Interfaces;

/// <summary>
/// This helper class is intended to contain the context specific rules evaluation necessary for each context defined in <see cref="CPromotionRuleEvaluationContext"/>
/// We may end up using specific methods for specific contexts, There might be duplicate conditions in each method but for now this is the simplest pattern while we
/// build out and discover all the context rules we will need to evaluate.
/// For example, currently we have a method for evaluating all promotion summary rules, but its possible we may need a checkout or app specific method that only
/// evaluates a subset of the summary rules based on that context.
/// </summary>
public interface IPromotionRulesEngineHelper
{
    /// <summary>
    /// This method will evaluate all the default promotion rules.
    /// </summary>
    /// <param name="promotion"></param>
    /// <param name="orderAmount"></param>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public Task<bool> EvaluateAllDefaultRules(Promotion promotion, decimal orderAmount, string? customerId = null);

    /// <summary>
    /// This method will only evaluate the transaction amount default rule, since we always want to issue refunds even if a promotion is expired or not longer active.
    /// </summary>
    /// <param name="promotion"></param>
    /// <param name="orderAmount"></param>
    /// <returns></returns>
    public Task<bool> EvaluateDefaultRulesForRefundContext(Promotion promotion, decimal orderAmount);

    /// <summary>
    /// This method will evaluate all the promotion summary rules.
    /// </summary>
    /// <param name="promotion"></param>
    /// <param name="promotionSummary"></param>
    /// <returns></returns>
    public Task<bool> EvaluateAllSummaryRules(Promotion promotion, PromotionSummary promotionSummary);
}