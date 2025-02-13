using PromotionsEngine.Domain.Enumerations;

namespace PromotionsEngine.Infrastructure.Enumerations;

public class CosmosDocumentTypeEnum : EnumerationBase
{
    public static readonly CosmosDocumentTypeEnum Merchant = new (1, nameof(Merchant));

    public static readonly CosmosDocumentTypeEnum Promotions = new(2, nameof(Promotions)); 

    public CosmosDocumentTypeEnum(int id, string name) : base(id, name)
    {
    }
}