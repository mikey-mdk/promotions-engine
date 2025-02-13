using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Requests;

/// <summary>
/// This request object is intended to provide the repo all the fields we can potentially query on. This object may grow over time.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetMerchantsQueryRequest
{
    /// <summary>
    /// This list of merchant Ids to query on.
    /// </summary>
    public List<string> MerchantIds { get; set; } = new List<string>();

    //Indicates that we only want to return active merchants.
    public bool Active { get; set; } = true;
}