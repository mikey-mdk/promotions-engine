using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Requests;

/// <summary>
/// The request object used to perform a partial document update. We will directly replace the provided values in the document.
/// This is much more efficient operationally compared to the alternative of looking up the document, updating the values, and then replacing the document.
/// </summary>
[ExcludeFromCodeCoverage]
public class PatchMerchantRequest
{
    /// <summary>
    /// The Id of the merchant.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The external MerchantId from the Contentful system.
    /// </summary>
    public string? ExternalMerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the merchant.
    /// </summary>
    public string? MerchantName { get; set; } = string.Empty;

    /// <summary>
    /// The description of the merchant.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// The type of merchant.
    /// Its unlikely this property will be utilized for MVP.
    /// </summary>
    public string? MerchantType { get; set; } = string.Empty;

    /// <summary>
    /// The type of business sector the merchant operates in.
    /// Its unlikely this property will be utilized for MVP.
    /// </summary>
    public string? BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// The physical address of the merchant.
    /// </summary>
    public PatchMerchantAddressRequest? MerchantAddress { get; set; }

    /// <summary>
    /// The list of regex patterns to match the merchant name.
    /// </summary>
    public List<string>? RegexPatterns { get; set; }

    /// <summary>
    /// Indicates if the Merchant record is currently active.
    /// </summary>
    public bool? Active { get; set; }
}