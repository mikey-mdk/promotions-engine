using System.Diagnostics.CodeAnalysis;
using Shouldly;
using Xunit;
using PromotionsEngine.Domain.Helpers.Implementations;
using PromotionsEngine.Domain.Helpers.Interfaces;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Tests.Domain.Helpers;

[ExcludeFromCodeCoverage]
public class PromotionRulesEngineHelperTests
{
    private readonly IPromotionRulesEngineHelper _helper;

    public PromotionRulesEngineHelperTests()
    {
        _helper = new PromotionRulesEngineHelper();
    }

    public static TheoryData<Promotion, decimal, string?, bool> DefaultRulesData = new()
    {
        {
            new Promotion //active false
            {
                Active = false
            },
            100m,
            null,
            false
        },
        {
            new Promotion //start date invalid
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(1)
            },
            100m,
            null,
            false
        },
        {
            new Promotion //end date invalid
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-10),
                PromotionEndDate = DateTime.UtcNow.AddDays(-1)
            },
            100m,
            null,
            false
        },
        {
            new Promotion //min transaction amount invalid
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                PromotionRules = new PromotionRules
                {
                    MinimumTransactionAmount = 100m
                }
            },
            90m,
            null,
            false
        },
        {
            new Promotion //max transaction amount invalid
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                PromotionRules = new PromotionRules
                {
                    MinimumTransactionAmount = 100m,
                    MaximumTransactionAmount = 200m
                }
            },
            201m,
            null,
            false
        },
        {
            new Promotion //transaction amount valid
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                PromotionRules = new PromotionRules
                {
                    MinimumTransactionAmount = 100m,
                    MaximumTransactionAmount = 200m
                }
            },
            110m,
            null,
            true
        },
        {
            new Promotion //transaction amount valid, no max
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                PromotionRules = new PromotionRules
                {
                    MinimumTransactionAmount = 100m
                }
            },
            110m,
            null,
            true
        },
        {
            new Promotion //no previous redemptions for customer
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                PromotionRules = new PromotionRules
                {
                    MinimumTransactionAmount = 100m
                },
                CustomerIds = new List<string> { "1", "2", "3" }
            },
            110m,
            "4",
            true
        },
        {
            new Promotion //customer already redeemed promotion
            {
                Active = true,
                PromotionStartDate = DateTime.UtcNow.AddDays(-1),
                PromotionEndDate = DateTime.UtcNow.AddDays(1),
                PromotionRules = new PromotionRules
                {
                    MinimumTransactionAmount = 100m
                },
                CustomerIds = new List<string> { "1", "2", "3" }
            },
            110m,
            "1",
            false
        }
    };

    public static TheoryData<Promotion, PromotionSummary, bool> SummaryRulesData = new()
    {
        {
            new Promotion //number of times redeemable invalid
            {
                PromotionRules = new PromotionRules
                {
                    NumberOfTimesRedeemable = 100
                }
            },
            new PromotionSummary
            {
                NumberOfTimesRedeemed = 100
            },
            false
        },
        {
            new Promotion //total amount redeemable invalid
            {
                PromotionRules = new PromotionRules
                {
                    TotalRewardsAmount = 1000
                }
            },
            new PromotionSummary
            {
                TotalAmountRedeemed = 1000
            },
            false
        },
        {
            new Promotion //total number of customers invalid
            {
                PromotionRules = new PromotionRules
                {
                    TotalNumberOfCustomers = 100
                }
            },
            new PromotionSummary
            {
                TotalNumberOfCustomers = 100
            },
            false
        },
        {
            new Promotion //all rules valid
            {
                PromotionRules = new PromotionRules
                {
                    NumberOfTimesRedeemable = 100,
                    TotalRewardsAmount = 1000,
                    TotalNumberOfCustomers = 100,
                }
            },
            new PromotionSummary
            {
                NumberOfTimesRedeemed = 99,
                TotalAmountRedeemed = 999,
                TotalNumberOfCustomers = 99
            },
            true
        }
    };
    
    [Theory]
    [MemberData(nameof(DefaultRulesData))]
    public async Task Test_Evaluate_All_Default_Rules(Promotion promotion, decimal orderAmount, string? customerId, bool expectedResult)
    {
        var result = await _helper.EvaluateAllDefaultRules(promotion, orderAmount, customerId);

        result.ShouldBe(expectedResult);
    }

    [Theory]
    [MemberData(nameof(SummaryRulesData))]
    public async Task Test_Evaluate_All_Summary_Rules(Promotion promotion, PromotionSummary promotionSummary, bool expectedResult)
    {
        var result = await _helper.EvaluateAllSummaryRules(promotion, promotionSummary);

        result.ShouldBe(expectedResult);
    }

    [Fact]
    public async Task Test_Evaluate_Default_Rules_For_Refund()
    {
        var result = await _helper.EvaluateDefaultRulesForRefundContext(new Promotion
        {
            PromotionRules = new PromotionRules
            {
                MinimumTransactionAmount = 100
            }
        }, 90);

        result.ShouldBeFalse();
    }
}