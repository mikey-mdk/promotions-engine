using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PromotionsEngine.Infrastructure.Entities;

[ExcludeFromCodeCoverage]
public class PromotionSummaryEntity
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    public int TotalNumberOfCustomers { get; set; }

    public int NumberOfTimesRedeemed { get; set; }

    public decimal TotalAmountRedeemed { get; set; }
}