using FluentAssertions;
using PromotionsEngine.Application.Dtos.Promotion;
using PromotionsEngine.Application.Dtos.PromotionRules;
using PromotionsEngine.Application.Mappers;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Tests.Mappers;

public class PromotionsDtoMapperTests
{
    [Fact]
    public void Should_map_NumberOfTimesRedeemableRuleDto_from_dto_to_domain()
    {
        // Arrange
        var dto = new PromotionRulesDto
        {
            NumberOfTimesRedeemable = 100,
            MinimumTransactionAmount = 150,
            MaximumTransactionAmount = 10000,
            NumberOfRedemptionsPerCustomer = 1,
            TotalRewardsAmount = 500000
        };

        // Act
        var domain = dto.MapToDomain();

        // Assert
        domain.ShouldNotBeNull();
        domain.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public void Should_map_promotion_from_domain_to_dto()
    {
        // Arrange
        var domain = new Promotion
        {
            Id = Guid.NewGuid().ToString(),
            MerchantId = "merchant-id",
            PromotionName = "promotion-name",
            PromotionRules = new PromotionRules
            {
                NumberOfTimesRedeemable = 500,
                MinimumTransactionAmount = 150
            },
            PromotionTypeEnum = PromotionTypeEnum.Cashback,
            PromotionDescription = "promotion-description",
            PromotionStartDate = DateTime.MinValue,
            PromotionEndDate = DateTime.MaxValue,
            RewardRateTypeEnum = RewardRateTypeEnum.Fixed,
            RateAmount = 123,
            Active = true,
            Deleted = false,
            CreatedDateTime = DateTime.UtcNow,
            ModifiedDateTime = DateTime.UtcNow,
            SchemaVersion = 0,
        };

        // Act
        var dto = domain.MapToDto();

        // Assert
        dto.Should().BeEquivalentTo(domain, options =>
            options.ExcludingMissingMembers());
    }
}