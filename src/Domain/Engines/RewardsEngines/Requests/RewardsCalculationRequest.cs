using PromotionsEngine.Domain.Enumerations;

namespace PromotionsEngine.Domain.Engines.RewardsEngines.Requests;

public class RewardsCalculationRequest
{
    public decimal OrderAmount { get; set; }

    public RewardRateTypeEnum RewardRateType { get; set; } = null!;

    public decimal RateAmount { get; set; }
}