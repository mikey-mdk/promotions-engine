using System.Diagnostics.CodeAnalysis;

namespace PromotionsEngine.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class CosmosDbOptions
{
    public static readonly string CosmosDbOptionsSectionName = "PromotionsEngineCosmosDb";

    public static readonly string CosmosClientName = "CosmosClient";

    public string DatabaseName { get; set; } = string.Empty;

    public string MerchantContainerName { get; set; } = string.Empty;

    public string MerchantLeaseContainerName { get; set; } = string.Empty;

    public string PromotionsLeaseContainerName { get; set; } = string.Empty;

    public string PromotionsContainerName { get; set; } = string.Empty;

    public string CustomerOrderRewardsLedgerContainerName { get; set; } = string.Empty;

    public string PromotionSummaryContainerName { get; set; } = string.Empty;

    public string MerchantRegexLookupContainerName { get; set; } = string.Empty;
}