using PromotionsEngine.Application.Dtos.Merchant;

namespace PromotionsEngine.Application.Requests.Merchant;

public class CreateMerchantRequest
{
    /// <summary>
    /// The external MerchantId from the Contentful system.
    /// </summary>
    public string ExternalMerchantId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the merchant.
    /// </summary>
    public string MerchantName { get; set; } = string.Empty;

    /// <summary>
    /// The description of the merchant.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The type of merchant.
    /// Its unlikely this property will be utilized for MVP.
    /// </summary>
    public string MerchantType { get; set; } = string.Empty;

    /// <summary>
    /// The type of business sector the merchant operates in.
    /// Its unlikely this property will be utilized for MVP.
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// The physical address of the merchant.
    /// </summary>
    public MerchantAddressDto MerchantAddress { get; set; } = new();

    /// <summary>
    /// Indicates if the Merchant record is currently active.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// The list of regex patterns that will be used to identify this merchant on incoming events.
    /// </summary>
    public List<string> RegexPatterns { get; set; } = new();
}