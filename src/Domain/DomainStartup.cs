using Microsoft.Extensions.DependencyInjection;
using PromotionsEngine.Domain.Engines.RewardsEngines.Implementations;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Implementations;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Helpers.Implementations;
using PromotionsEngine.Domain.Helpers.Interfaces;

namespace PromotionsEngine.Domain;

public static class DomainStartup
{
    public static IServiceCollection AddDomainStartup(this IServiceCollection services)
    {
        services.AddTransient<IPromotionRulesEngine, PromotionRulesEngine>();
        services.AddTransient<IRewardsCalculationEngine, RewardsCalculationEngine>();
        services.AddTransient<IPromotionRulesEngineHelper, PromotionRulesEngineHelper>();

        return services;
    }
}