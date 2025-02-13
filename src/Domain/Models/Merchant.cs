using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Domain.Models;

[ExcludeFromCodeCoverage]
public class Merchant
{
    /// <summary>
    /// The unique identifier for the Merchant in the Cosmos container.
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
    /// <see cref="CMerchantTypes"/>
    /// </summary>
    public string MerchantType { get; set; } = string.Empty;

    /// <summary>
    /// The type of business sector the merchant operates in.
    /// Its unlikely this property will be utilized for MVP.
    /// <see cref="CBusinessTypes"/>
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// The physical address of the merchant.
    /// </summary>
    public MerchantAddress MerchantAddress { get; set; } = new();

    /// <summary>
    /// This will be the list of regex patterns that we use to match the merchant name to the merchant Id.
    /// This is necessary because the incoming events will not have unique merchant Ids.
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
    public bool Deleted { get; set; } = false;

    /// <summary>
    /// Indicates if the Merchant record is currently active.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// The current schema version of the Merchant document in CosmosDB that the App is supporting.
    /// </summary>
    public int SchemaVersion { get; set; }
}