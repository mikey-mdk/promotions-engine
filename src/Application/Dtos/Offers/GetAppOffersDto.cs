using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PromotionsEngine.Application.Dtos.Offers;

[ExcludeFromCodeCoverage]
public class GetAppOffersDto
{
    [JsonPropertyName("ResultList")]
    public List<AppOfferDto> AppOfferDtos { get; set; } = new();

    /// <summary>
    /// Indicates if there are more results to be fetched.
    /// This property is being added for backward compatibility to the Beam API but will likely not be leveraged in this version.
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Used to identify individual requests. This is being added for backward compatibility but is unused currently.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}