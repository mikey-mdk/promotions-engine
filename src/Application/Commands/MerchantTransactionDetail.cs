namespace PromotionsEngine.Application.Commands;

public class MerchantTransactionDetail
{
    /// <summary>
    /// Merchant/Location Name.
    /// </summary>
    public string MerchantName { get; init; } = string.Empty;

    /// <summary>
    /// Sum captured amount for the specified merchant.
    /// </summary>
    public decimal TotalGatewayCaptured { get; init; }

    /// <summary>
    /// Sum refunded amount for the specified merchant.
    /// </summary>
    public decimal TotalGatewayRefunded { get; init; }
}
