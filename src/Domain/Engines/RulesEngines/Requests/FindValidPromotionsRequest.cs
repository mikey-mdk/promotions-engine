using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Domain.Engines.RulesEngines.Requests;

[ExcludeFromCodeCoverage]
public class FindValidPromotionsRequest
{
    /// <summary>
    /// The list of promotions to be evaluated.
    /// </summary>
    public List<Promotion> Promotions { get; set; } = new();

    /// <summary>
    /// The order amount to validate the promotions against.
    /// </summary>
    public decimal OrderAmount { get; set; }

    /// <summary>
    /// If provided, checks if a promotion has been redeemed by this customer. Not all work flows will require this which is why it is nullable.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// <see cref="CPromotionRuleEvaluationContext"/>
    /// </summary>
    public string EvaluationContext { get; set; } = string.Empty;
}