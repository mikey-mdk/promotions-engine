using FluentAssertions;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;

namespace PromotionsEngine.Infrastructure.Tests.Mappers;

public class PromotionsEntityMapperTests
{
    [Fact]
    public void Should_map_promotion_from_entity_to_domain()
    {
        // Arrange
        var entity = new PromotionEntity
        {
            Id = Guid.NewGuid().ToString(),
            MerchantId = "merchant-id",
            PromotionName = "promotion-name",
            PromotionRules = new PromotionRulesEntity
            {
                NumberOfTimesRedeemable = 100,
                MinimumTransactionAmount = 100,
                MaximumTransactionAmount = 200,
                TotalRewardsAmount = 10000,
                NumberOfRedemptionsPerCustomer = 1
            },
            PromotionTypeEnum = PromotionTypeEnum.Cashback.Id,
            PromotionDescription = "promotion-description",
            PromotionStartDate = DateTime.MinValue,
            PromotionEndDate = DateTime.MaxValue,
            RewardRateTypeEnum = RewardRateTypeEnum.Fixed.Id,
            RateAmount = 123,
            Active = true,
            Deleted = false,
            CreatedDateTime = DateTime.UtcNow,
            ModifiedDateTime = DateTime.UtcNow,
            SchemaVersion = 0,
        };

        // Act
        var domain = entity.MapToDomain();

        // Assert
        domain.Should().BeEquivalentTo(entity, options => 
            options
                .Excluding(x => x.PromotionTypeEnum)
                .Excluding(x => x.RewardRateTypeEnum));
        entity.PromotionTypeEnum.ShouldBe(domain.PromotionTypeEnum!.Id);
        entity.RewardRateTypeEnum.ShouldBe(domain.RewardRateTypeEnum!.Id);
    }

    [Fact]
    public void Should_map_promotion_from_domain_to_entity()
    {
        // Arrange
        var domain = new Promotion
        {
            Id = Guid.NewGuid().ToString(),
            MerchantId = "merchant-id",
            PromotionName = "promotion-name",
            PromotionRules = new PromotionRules
            {
                NumberOfTimesRedeemable = 100,
                MinimumTransactionAmount = 100,
                MaximumTransactionAmount = 200,
                TotalRewardsAmount = 10000,
                NumberOfRedemptionsPerCustomer = 1
            },
            PromotionTypeEnum = PromotionTypeEnum.Cashback,
            PromotionDescription = "promotion-description",
            PromotionStartDate = DateTime.MinValue.Date,
            PromotionEndDate = DateTime.MaxValue.Date,
            RewardRateTypeEnum = RewardRateTypeEnum.Fixed,
            RateAmount = 123,
            Active = true,
            Deleted = false,
            CreatedDateTime = DateTime.UtcNow,
            ModifiedDateTime = DateTime.UtcNow,
            SchemaVersion = 0,
        };

        // Act
        var entity = domain.MapToEntity();

        // Assert
        entity.Should().BeEquivalentTo(domain, options =>
            options
                .ExcludingMissingMembers()
                .Excluding(x => x.PromotionTypeEnum)
                .Excluding(x => x.RewardRateTypeEnum));
        entity.PromotionTypeEnum.ShouldBe(domain.PromotionTypeEnum.Id);
        entity.RewardRateTypeEnum.ShouldBe(domain.RewardRateTypeEnum.Id);
    }
}