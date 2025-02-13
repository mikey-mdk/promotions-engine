using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Domain.Enumerations;

[ExcludeFromCodeCoverage]
public class BusinessTypeEnum : EnumerationBase
{
    public static readonly BusinessTypeEnum Electronics = new (1, nameof(Electronics));

    public static readonly BusinessTypeEnum Fashion = new(2, nameof(Fashion)); 

    public BusinessTypeEnum(int id, string name) : base(id, name)
    {
    }
}