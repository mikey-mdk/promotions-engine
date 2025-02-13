using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Requests;

[ExcludeFromCodeCoverage]
public class GetPromotionsQueryRequest
{
    public string MerchantId { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool Active { get; set; }
}