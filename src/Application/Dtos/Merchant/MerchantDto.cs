using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Application.Dtos.Merchant;

[ExcludeFromCodeCoverage]
public class MerchantDto
{
    /// <summary>
    /// The unique identifier of the merchant document in the cosmos db container.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The internal Id that will be used to uniquely identify a merchant record
    /// </summary>
    public string MerchantId { get; set; } = string.Empty;

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
    /// The regex patterns that are used to identify the merchant via the merchant name.
    /// </summary>
    public List<string> RegexPatterns { get; set; } = new();

    /// <summary>
    /// The DateTime the Merchant record was created.
    /// </summary>
    public DateTime CreatedDateTime { get; set; }

    /// <summary>
    /// The DateTime of the most recent modification to the Merchant Record.
    /// </summary>
    public DateTime? ModifiedDateTime { get; set; }

    /// <summary>
    /// Indicates if the Merchant record has been deleted.
    /// </summary>
    public bool? Deleted { get; set; }

    /// <summary>
    /// Indicates if the Merchant record is currently active.
    /// </summary>
    public bool? Active { get; set; }
}