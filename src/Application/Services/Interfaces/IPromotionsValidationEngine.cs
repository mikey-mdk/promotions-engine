using PromotionsEngine.Application.Requests.Promotions;

namespace PromotionsEngine.Application.Services.Interfaces;

public interface IPromotionsValidationEngine
{
    Task Validate(CreatePromotionRequest request);

    Task Validate(UpdatePromotionRequest request);
}
