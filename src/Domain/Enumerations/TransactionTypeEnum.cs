using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class TransactionTypeEnum : EnumerationBase
{
    public static readonly TransactionTypeEnum OrderCreated = new(1, nameof(OrderCreated));

    public static readonly TransactionTypeEnum OrderRefunded = new(2, nameof(OrderRefunded));

    public static readonly TransactionTypeEnum OrderSettled = new(3, nameof(OrderSettled));

    public static readonly TransactionTypeEnum Unknown = new(0, nameof(Unknown));

    public TransactionTypeEnum(int id, string name) : base(id, name)
    {
    }
}