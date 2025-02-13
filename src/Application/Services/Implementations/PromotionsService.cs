using PromotionsEngine.Application.Dtos.Promotion;
using PromotionsEngine.Application.Mappers;
using PromotionsEngine.Application.Requests.Promotions;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Application.Services.Implementations;

public class PromotionsService : IPromotionsService
{
    private readonly IPromotionsRepository _repo;
    private readonly IPromotionsValidationEngine _validator;

    public PromotionsService(
        IPromotionsRepository repo,
        IPromotionsValidationEngine validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<PromotionDto> GetPromotionByIdAsync(string promotionId, CancellationToken cancellationToken)
    {
        var promotion = await _repo.GetPromotionByIdAsync(promotionId, cancellationToken);
        return promotion.MapToDto();
    }

    public async Task<PromotionDto> CreatePromotionAsync(CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        await _validator.Validate(request);

        var newPromotion = new Promotion
        {
            Id = Guid.NewGuid().ToString(),
            CreatedDateTime = DateTime.UtcNow,
            ModifiedDateTime = DateTime.UtcNow,

            // Build from the create request:
            MerchantId = request.MerchantId,
            PromotionName = request.PromotionName,
            PromotionRules = request.PromotionRules.MapToDomain(),
            PromotionTypeEnum = EnumerationBase.GetAll<PromotionTypeEnum>().FirstOrDefault(x => x.Id == request.PromotionTypeEnum),
            PromotionDescription = request.PromotionDescription,
            PromotionStartDate = request.PromotionStartDate,
            PromotionEndDate = request.PromotionEndDate,
            RewardRateTypeEnum = EnumerationBase.GetAll<RewardRateTypeEnum>().FirstOrDefault(x => x.Id == request.RewardRateTypeEnum),
            RateAmount = request.RateAmount
        };

        var createdPromotion = await _repo.CreatePromotionAsync(newPromotion, cancellationToken);
        return createdPromotion.MapToDto();
    }

    public async Task<List<PromotionDto>> GetPromotionsFromQueryAsync(GetPromotionsQueryRequest request, CancellationToken cancellationToken)
    {
        var results = await _repo.GetPromotionsFromQueryAsync(request, cancellationToken);

        return results.Select(x => x.MapToDto()).ToList();
    }

    public async Task<PromotionDto> UpdatePromotionAsync(UpdatePromotionRequest request, CancellationToken cancellationToken)
    {
        await _validator.Validate(request);

        var promotion = await _repo.GetPromotionByIdAsync(request.Id, cancellationToken);

        // Update from the request:
        promotion.MerchantId = request.MerchantId;
        promotion.PromotionName = request.PromotionName;
        promotion.PromotionRules = request.PromotionRules.MapToDomain();
        promotion.PromotionTypeEnum = EnumerationBase.GetAll<PromotionTypeEnum>().FirstOrDefault(x => x.Name == request.PromotionType);
        promotion.PromotionDescription = request.PromotionDescription;
        promotion.PromotionStartDate = request.PromotionStartDate;
        promotion.PromotionEndDate = request.PromotionEndDate;
        promotion.RewardRateTypeEnum = EnumerationBase.GetAll<RewardRateTypeEnum>().FirstOrDefault(x => x.Name == request.RewardRateType);
        promotion.RateAmount = request.RateAmount;

        // Update Metadata:
        promotion.ModifiedDateTime = DateTime.UtcNow;

        var updatedPromotion = await _repo.UpdatePromotionAsync(promotion, cancellationToken);
        return updatedPromotion.MapToDto();
    }

    public async Task<PromotionDto> DeletePromotionAsync(string promotionId, CancellationToken cancellationToken)
    {
        var promotion = await _repo.DeletePromotionAsync(promotionId, cancellationToken);
        return promotion.MapToDto();
    }
}
