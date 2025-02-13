using Microsoft.Extensions.DependencyInjection;
using PromotionsEngine.Application.Cache.Implementations;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Engines.Implementations;
using PromotionsEngine.Application.Engines.Interfaces;
using PromotionsEngine.Application.Managers.Implementations;
using PromotionsEngine.Application.Managers.Interfaces;
using PromotionsEngine.Application.QueryHandlers.Implementations;
using PromotionsEngine.Application.QueryHandlers.Interfaces;
using PromotionsEngine.Application.Services.Implementations;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Application.Validation.Implementations;
using PromotionsEngine.Application.Validation.Interfaces;

namespace PromotionsEngine.Application;

public static class ApplicationStartup
{
    public static IServiceCollection AddApplicationStartup(this IServiceCollection services)
    {
        services.AddTransient<IServiceBusManager, ServiceBusManager>();
        services.AddTransient<IMerchantService, MerchantService>();
        services.AddTransient<IMerchantValidationEngine, MerchantValidationEngine>();
        services.AddTransient<IPromotionsService, PromotionsService>();
        services.AddTransient<IPromotionsValidationEngine, PromotionsValidationEngine>();
        services.AddTransient<IGetOffersForCheckoutQueryHandler, GetOffersForCheckoutQueryHandler>();
        services.AddTransient<IGetOffersForAppQueryHandler, GetOffersForAppQueryHandler>();
        services.AddTransient<IRedisCacheManager, RedisCacheManager>();
        services.AddTransient<IMerchantRegexLookupCacheManager, MerchantRegexLookupCacheManager>();
        services.AddTransient<IMerchantIdentificationService, MerchantIdentificationService>();
        services.AddTransient<IRegexEvaluationEngine, RegexEvaluationEngine>();
        services.AddTransient<IRewardsDistributionReconciliationService, RewardsDistributionReconciliationService>();

        return services;
    }
}