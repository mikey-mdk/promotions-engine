using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Models;

[ExcludeFromCodeCoverage]
public class MerchantRegex
{

    /// <summary>
    /// The internal Id of the merchant document in Cosmos.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The regex patterns to match the merchant name.
    /// </summary>
    public List<string> RegexPatterns { get; set; } = new();
}