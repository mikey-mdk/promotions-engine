using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Xunit;
using PromotionsEngine.Domain.Engines.RewardsEngines.Implementations;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Tests.Domain.Engines;

[ExcludeFromCodeCoverage]
public class RewardsCalculationEngineTests
{
    private readonly IRewardsCalculationEngine _rewardsCalculationEngine;

    public RewardsCalculationEngineTests()
    {
        _rewardsCalculationEngine = new RewardsCalculationEngine();
    }

    public static TheoryData<decimal, decimal, decimal, RewardRateTypeEnum> Data = new()
    {
        {100m, 10m, 10m, RewardRateTypeEnum.Fixed},
        {100m, 5m, 5m, RewardRateTypeEnum.Percentage},
        {233m, 10m, 10m, RewardRateTypeEnum.Fixed},
        {233m, 5m, 11.65m, RewardRateTypeEnum.Percentage}
    };

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", "RewardsCalculationEngine")]
    [Trait("Method", "FindLargestRewardForOrderAsync")]
    [Description("Test the highest reward is returned from a list of promotions")]
    public async Task Test_Find_Largest_Reward_For_Order_Async()
    {
        var orderId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();
        var promotionOneId = Guid.NewGuid().ToString();
        var promotionTwoId = Guid.NewGuid().ToString();
        var promotionThreeId = Guid.NewGuid().ToString();
        var orderAmount = 100m;
        var rateAmountOne = 10m;
        var rateAmountTwo = 5m;
        var rateAmountThree = 7m;
        var expectedAmount = 10m;

        var promotionOne = new Promotion
        {
            Id = promotionOneId,
            RewardRateTypeEnum = RewardRateTypeEnum.Fixed,
            RateAmount = rateAmountOne,
        };

        var promotionTwo = new Promotion
        {
            Id = promotionTwoId,
            RewardRateTypeEnum = RewardRateTypeEnum.Percentage,
            RateAmount = rateAmountTwo
        };

        var promotionThree = new Promotion
        {
            Id = promotionThreeId,
            RewardRateTypeEnum = RewardRateTypeEnum.Percentage,
            RateAmount = rateAmountThree
        };

        var expectedReward = new Reward
        {
            Amount = expectedAmount,
            OrderId = orderId,
            CustomerId = customerId,
            PromotionId = promotionOneId
        };

        var result = await _rewardsCalculationEngine.FindLargestRewardForOrderAsync(new FindLargestRewardForOrderRequest
        {
            OrderId = orderId,
            CustomerId = customerId,
            Amount = orderAmount,
            Promotions = new()
            {
                promotionOne,
                promotionTwo,
                promotionThree
            }
        });

        result.ShouldBeEquivalentTo(expectedReward);
    }

    [Theory]
    [MemberData(nameof(Data))]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", "RewardsCalculationEngine")]
    [Trait("Method", "CalculateRewardsAsync")]
    [Description("Test various scenarios for calculating reward amounts")]
    public async Task Test_Calculate_Rewards_Async(decimal orderAmount, decimal rateAmount, decimal expectedAmount, RewardRateTypeEnum rateType)
    {
        var rewardAmount = await _rewardsCalculationEngine.CalculateRewardsAsync(new RewardsCalculationRequest
        {
            OrderAmount = orderAmount,
            RateAmount = rateAmount,
            RewardRateType = rateType
        });

        rewardAmount.ShouldBeEquivalentTo(expectedAmount);
    }
}