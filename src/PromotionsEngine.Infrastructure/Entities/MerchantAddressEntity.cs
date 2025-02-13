using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Infrastructure.Entities;

[ExcludeFromCodeCoverage]
public class MerchantAddressEntity
{
    public string AddressLine1 { get; set; } = string.Empty;

    public string AddressLine2 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}