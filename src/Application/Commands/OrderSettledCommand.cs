using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Constants;

namespace PromotionsEngine.Application.Commands;

[ExcludeFromCodeCoverage]
public partial class OrderSettledCommand : CommandBase
{
    /// <summary>
    /// The originial type of this message from <see cref="BusMessages.PromotionsEngineTransactionMessage"/>.
    /// </summary>
    public override string TransactionType { get; } = CTransactionType.OrderSettled;

    /// <summary>
    /// Order Id.
    /// </summary>
    public string OrderId { get; init; } = string.Empty;

    /// <summary>
    /// Aggregated transaction details per merchant (where available).
    /// </summary>
    public IEnumerable<MerchantTransactionDetail> TransactionDetails { get; init; } = new List<MerchantTransactionDetail>();
}
