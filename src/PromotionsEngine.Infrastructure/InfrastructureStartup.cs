using Microsoft.Extensions.DependencyInjection;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Infrastructure.ChangeFeed.Implementations;
using PromotionsEngine.Infrastructure.ChangeFeed.Interfaces;
using PromotionsEngine.Infrastructure.Repositories.Implementations;

namespace PromotionsEngine.Infrastructure;

public static class InfrastructureStartup
{
    public static IServiceCollection AddInfrastructureStartup(this IServiceCollection services)
    {
        services.AddTransient<IMerchantRepository, MerchantRepository>();
        services.AddTransient<IPromotionsRepository, PromotionsRepository>();
        services.AddTransient<ICustomerOrderRewardsLedgerRepository, CustomerOrderRewardsLedgerRepository>();
        services.AddTransient<IPromotionSummaryRepository, PromotionSummaryRepository>();
        services.AddSingleton<ICosmosChangeFeedProcessor, CosmosChangeFeedProcessor>();
        services.AddTransient<IMerchantRegexRepository, MerchantRegexRepository>();

        return services;
    }
}