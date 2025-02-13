using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.QueryHandlers.Implementations;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Models;
using AutoFixture;
using FluentAssertions;
using PromotionsEngine.Application.Dtos.Offers;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Application.Queries;

namespace PromotionsEngine.Tests.Application.QueryHandlers;

[ExcludeFromCodeCoverage]
public class GetOffersForAppQueryHandlerTests
{
    private readonly IMerchantRepository _fakeMerchantRepository;
    private readonly IPromotionsRepository _fakePromotionsRepository;
    private readonly IRedisCacheManager _fakeRedisCacheManager;
    private readonly Fixture _fixture;

    private readonly GetOffersForAppQueryHandler _queryHandler;

    public GetOffersForAppQueryHandlerTests()
    {
        _fakeMerchantRepository = A.Fake<IMerchantRepository>();
        _fakePromotionsRepository = A.Fake<IPromotionsRepository>();
        _fakeRedisCacheManager = A.Fake<IRedisCacheManager>();
        _fixture = new Fixture();

        _queryHandler = new GetOffersForAppQueryHandler(
            _fakeRedisCacheManager,
            _fakePromotionsRepository,
            _fakeMerchantRepository);
    }

    [Fact]
    [Trait("Class", nameof(GetOffersForAppQueryHandler))]
    [Trait("Category", "Unit")]
    [Trait("Endpoint", "GetOffersForCheckoutAsync")]
    [Description($"Test the happy path for {nameof(GetOffersForAppQueryHandler)}")]
    public async Task Test_Get_Offers_For_Checkout_Happy_Path()
    {
        var merchantOne = _fixture.Create<Merchant>();
        var merchantTwo = _fixture.Create<Merchant>();

        var now = DateTime.UtcNow.Date;

        var promotionOne = _fixture.Build<Promotion>().With(p => p.MerchantId, merchantOne.MerchantId)
            .With(p => p.RewardRateTypeEnum, RewardRateTypeEnum.Percentage)
            .With(p => p.PromotionStartDate, now).Create();
        var promotionTwo = _fixture.Build<Promotion>().With(p => p.MerchantId, merchantOne.MerchantId)
            .With(p => p.RewardRateTypeEnum, RewardRateTypeEnum.Percentage)
            .With(p => p.PromotionStartDate, now).Create();
        var promotionThree = _fixture.Build<Promotion>().With(p => p.MerchantId, merchantTwo.MerchantId)
            .With(p => p.RewardRateTypeEnum, RewardRateTypeEnum.Fixed)
            .With(p => p.PromotionStartDate, now).Create();

        var expectedOfferOne = new AppOfferDto
        {
            ExternalMerchantId = merchantOne.ExternalMerchantId,
            StartDate = promotionOne.PromotionStartDate,
            Name = promotionOne.PromotionName,
            RatePercentage = promotionOne.RateAmount,
            RateFixed = 0m,
            Type = promotionOne.PromotionTypeEnum!.Name,
        };

        var expectedOfferTwo = new AppOfferDto
        {
            ExternalMerchantId = merchantOne.ExternalMerchantId,
            StartDate = promotionTwo.PromotionStartDate,
            Name = promotionTwo.PromotionName,
            RatePercentage = promotionTwo.RateAmount,
            RateFixed = 0m,
            Type = promotionTwo.PromotionTypeEnum!.Name,
        };

        var expectedOfferThree = new AppOfferDto
        {
            ExternalMerchantId = merchantTwo.ExternalMerchantId,
            StartDate = promotionThree.PromotionStartDate,
            Name = promotionThree.PromotionName,
            RateFixed = promotionThree.RateAmount,
            RatePercentage = 0m,
            Type = promotionThree.PromotionTypeEnum!.Name,
        };

        var redisCall = A.CallTo(() =>
            _fakeRedisCacheManager.GetOrSetAsync(A<string>._, A<Func<Task<ValueTuple<List<Merchant>, List<Promotion>>>>>._));
        redisCall.Returns(ValueTuple.Create(new List<Merchant> { merchantOne, merchantTwo },
            new List<Promotion> { promotionOne, promotionTwo, promotionThree }));

        var result = await _queryHandler.GetOffersForAppAsync(new GetOffersForAppQuery(), CancellationToken.None);

        result.AppOfferDtos.Should().ContainEquivalentOf(expectedOfferOne);
        result.AppOfferDtos.Should().ContainEquivalentOf(expectedOfferTwo);
        result.AppOfferDtos.Should().ContainEquivalentOf(expectedOfferThree);
        
        redisCall.MustHaveHappenedOnceExactly();
    }
}