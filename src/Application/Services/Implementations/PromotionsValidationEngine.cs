using PromotionsEngine.Application.Requests.Promotions;
using PromotionsEngine.Application.Services.Interfaces;

namespace PromotionsEngine.Application.Services.Implementations;

public class PromotionsValidationEngine : IPromotionsValidationEngine
{
    public PromotionsValidationEngine()
    {
    }

    public Task Validate(CreatePromotionRequest request)
    {
        return Task.CompletedTask;
    }

    public Task Validate(UpdatePromotionRequest request)
    {
        return Task.CompletedTask;
    }
}