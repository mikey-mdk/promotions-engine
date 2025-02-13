using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Application.Queries;
using PromotionsEngine.Application.QueryHandlers.Implementations;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RewardsEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RewardsEngines.Requests;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Application.QueryHandlers;

[ExcludeFromCodeCoverage]
public class GetOffersForCheckoutQueryHandlerTests
{
    private readonly IMerchantRepository _fakeMerchantRepository;
    private readonly IPromotionsRepository _fakePromotionsRepository;
    private readonly IPromotionRulesEngine _fakePromotionRulesEngine;
    private readonly IRewardsCalculationEngine _fakeRewardsCalculationEngine;
    private readonly IRedisCacheManager _fakeRedisCacheManager;
    private readonly ILogger<GetOffersForCheckoutQueryHandler> _fakeLogger;

    private readonly GetOffersForCheckoutQueryHandler _queryHandler;

    public GetOffersForCheckoutQueryHandlerTests()
    {
        _fakeMerchantRepository = A.Fake<IMerchantRepository>();
        _fakePromotionsRepository = A.Fake<IPromotionsRepository>();
        _fakePromotionRulesEngine = A.Fake<IPromotionRulesEngine>();
        _fakeRewardsCalculationEngine = A.Fake<IRewardsCalculationEngine>();
        _fakeRedisCacheManager = A.Fake<IRedisCacheManager>();
        _fakeLogger = A.Fake<ILogger<GetOffersForCheckoutQueryHandler>>();

        _queryHandler = new GetOffersForCheckoutQueryHandler(
            _fakeMerchantRepository,
            _fakePromotionsRepository,
            _fakePromotionRulesEngine,
            _fakeRewardsCalculationEngine,
            _fakeRedisCacheManager,
            _fakeLogger);
    }

    [Fact]
    [Trait("Class", "GetOffersForCheckoutQueryHandler")]
    [Trait("Category", "Unit")]
    [Trait("Endpoint", "GetOffersForCheckoutAsync")]
    [Description("Test the happy path for GetOffersForCheckoutQueryHandler")]
    public async Task Test_Get_Offers_For_Checkout_Happy_Path()
    {
        const decimal orderAmount = 100;
        const decimal discountAmount = 10;
        const string merchantId = "testMerchantId";
        const string externalMerchantId = "externalMerchantId";
        const string merchantName = "testMerchantName";
        const string promotionOneName = "PromotionOne";
        const string promotionTwoName = "PromotionTwo";
        const string promotionOneDesc = "PromotionOneDesc";
        const string promotionTwoDesc = "PromotionTwoDesc";

        var startDate = DateTime.UtcNow.AddDays(-1);
        var endDate = DateTime.UtcNow.AddDays(1);


        var merchant = new Merchant
        {
            MerchantName = merchantName,
            MerchantId = merchantId,
            ExternalMerchantId = externalMerchantId
        };

        var promotionOne = new Promotion
        {
            PromotionName = promotionOneName,
            PromotionDescription = promotionOneDesc,
            PromotionStartDate = startDate,
            PromotionEndDate = endDate,
            Active = true
        };

        var promotionTwo = new Promotion
        {
            PromotionName = promotionTwoName,
            PromotionDescription = promotionTwoDesc,
            PromotionStartDate = startDate,
            PromotionEndDate = endDate,
            Active = true
        };

        var promotionThree = new Promotion
        {
            PromotionName = "PromotionThree",
            PromotionDescription = "PromotionDescription",
            PromotionStartDate = startDate,
            PromotionEndDate = endDate,
            Active = false
        };

        var expectedResponse = new GetCheckoutOffersDto
        {
            CheckoutOffers = new List<CheckoutOfferDto>
            {
                new()
                {
                    OrderAmount = orderAmount,
                    StartDate = startDate,
                    EndDate = endDate,
                    DiscountAmount = discountAmount,
                    MerchantId = merchantId,
                    MerchantName = merchantName,
                    ExternalMerchantId = externalMerchantId,
                    PromotionName = promotionOneName,
                    PromotionDescription = promotionOneDesc
                },
                new()
                {
                    OrderAmount = orderAmount,
                    StartDate = startDate,
                    EndDate = endDate,
                    DiscountAmount = discountAmount,
                    MerchantId = merchantId,
                    MerchantName = merchantName,
                    ExternalMerchantId = externalMerchantId,
                    PromotionName = promotionTwoName,
                    PromotionDescription = promotionTwoDesc
                }
            }
        };

        var redisCall = A.CallTo(() =>
            _fakeRedisCacheManager.GetOrSetAsync(A<string>._, A<Func<Task<ValueTuple<Merchant, List<Promotion>>>>>._));
        redisCall.Returns(ValueTuple.Create(merchant, new List<Promotion> { promotionOne, promotionTwo, promotionThree }));

        var rulesEngineResponse = new List<ValueTuple<Promotion, PromotionSummary>>
        {
            ValueTuple.Create(promotionOne, new PromotionSummary()),
            ValueTuple.Create(promotionTwo, new PromotionSummary())
        };

        var promotionRulesEngineCall = A.CallTo(() =>
            _fakePromotionRulesEngine.FindValidPromotions(A<FindValidPromotionsRequest>.That.Matches(x =>
                x.EvaluationContext == CPromotionRuleEvaluationContext.CheckoutPresentation)));
        promotionRulesEngineCall.Returns(rulesEngineResponse);

        var rewardsCalculationEngineCall = A.CallTo(() =>
            _fakeRewardsCalculationEngine.CalculateRewardsAsync(A<RewardsCalculationRequest>._));
        rewardsCalculationEngineCall.Returns(discountAmount);

        var result = await _queryHandler.GetOffersForCheckoutAsync(new GetOffersForCheckoutQuery
        {
            OrderAmount = orderAmount,
            MerchantId = merchantId
        }, CancellationToken.None);

        result.Should().BeEquivalentTo(expectedResponse);

        redisCall.MustHaveHappenedOnceExactly();
        promotionRulesEngineCall.MustHaveHappenedOnceExactly();
        rewardsCalculationEngineCall.MustHaveHappenedANumberOfTimesMatching(x => x == 2);
    }
}