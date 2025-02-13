using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Infrastructure.Entities;

/// <summary>
/// This entity is a sub entity of MerchantRegexLookupEntity. It contains the regex pattern to match the merchant name.
/// </summary>
[ExcludeFromCodeCoverage]
public class MerchantRegexEntity : EntityBase
{
    /// <summary>
    /// The regex patterns to match the merchant name.
    /// </summary>
    public List<string> RegexPatterns { get; set; } = new();
}