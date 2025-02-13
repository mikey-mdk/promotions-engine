using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PromotionsEngine.Application.Dtos.PromotionRules;
using PromotionsEngine.Application.Requests.Promotions;
using PromotionsEngine.Application.Services.Implementations;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Tests.Application.Services;

public class PromotionsServiceTests
{
    private readonly IPromotionsRepository _fakePromotionsRepository;
    private readonly IPromotionsValidationEngine _fakePromotionsValidationEngine;

    private PromotionsService Service => new(_fakePromotionsRepository, _fakePromotionsValidationEngine);

    public PromotionsServiceTests()
    {
        _fakePromotionsRepository = A.Fake<IPromotionsRepository>();
        _fakePromotionsValidationEngine = A.Fake<IPromotionsValidationEngine>();
    }

    [Fact]
    public async Task Should_get_by_id()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var promotion = new Promotion { Id = id };
        var repoCall = A.CallTo(() => _fakePromotionsRepository.GetPromotionByIdAsync(id, default));
        repoCall.Returns(promotion);

        // Act
        var response = await Service.GetPromotionByIdAsync(id, default);

        // Assert
        repoCall.MustHaveHappenedOnceExactly();
        response.Id.ShouldBe(id);
    }

    [Fact]
    public async Task Should_delete_by_id()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var promotion = new Promotion { Id = id };
        var repoCall = A.CallTo(() => _fakePromotionsRepository.DeletePromotionAsync(id, default));
        repoCall.Returns(promotion);

        // Act
        var response = await Service.DeletePromotionAsync(id, default);

        // Assert
        repoCall.MustHaveHappenedOnceExactly();
        response.Id.ShouldBe(id);
    }

    [Fact]
    public async Task Should_update_by_id()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var request = new UpdatePromotionRequest
        {
            Id = id,
            MerchantId = "merchant-id",
            PromotionName = "promotion-name",
            PromotionRules = new PromotionRulesDto
            {
                NumberOfTimesRedeemable = 100,
                MinimumTransactionAmount = 150
            },
            PromotionType = PromotionTypeEnum.Cashback.Name,
            PromotionDescription = "promotion-description",
            PromotionStartDate = DateTime.MinValue,
            PromotionEndDate = DateTime.MaxValue,
            RewardRateType = RewardRateTypeEnum.Fixed.Name,
            RateAmount = 123,
        };
        var promotion = new Promotion { Id = id };

        var validateCall = A.CallTo(() => _fakePromotionsValidationEngine.Validate(request));

        var getCall = A.CallTo(() => _fakePromotionsRepository.GetPromotionByIdAsync(id, default));
        getCall.Returns(promotion);

        var updatedPromotion = new Promotion();
        var updateCall = A.CallTo(() => _fakePromotionsRepository.UpdatePromotionAsync(promotion, default));
        updateCall.Invokes((Promotion x, CancellationToken _) => updatedPromotion = x);
        updateCall.ReturnsLazily(() => updatedPromotion);


        // Act
        var response = await Service.UpdatePromotionAsync(request, default);

        // Assert
        validateCall.MustHaveHappenedOnceExactly();
        updateCall.MustHaveHappenedOnceExactly();

        response.Id.ShouldBe(id);

        updatedPromotion.MerchantId.ShouldBe(request.MerchantId);
        updatedPromotion.PromotionName.ShouldBe(request.PromotionName);
        updatedPromotion.PromotionRules.Should().BeEquivalentTo(request.PromotionRules);
        updatedPromotion.PromotionTypeEnum.ShouldBe(PromotionTypeEnum.Cashback);
        updatedPromotion.PromotionDescription.ShouldBe(request.PromotionDescription);
        updatedPromotion.PromotionStartDate.ShouldBe(request.PromotionStartDate);
        updatedPromotion.PromotionEndDate.ShouldBe(request.PromotionEndDate);
        updatedPromotion.RewardRateTypeEnum.ShouldBe(RewardRateTypeEnum.Fixed);
        updatedPromotion.RateAmount.ShouldBe(request.RateAmount);
    }

    [Fact]
    public async Task Should_create()
    {
        // Arrange
        var request = new CreatePromotionRequest
        {
            MerchantId = "merchant-id",
            PromotionName = "promotion-name",
            PromotionRules = new PromotionRulesDto
            {
                NumberOfTimesRedeemable = 500,
                MinimumTransactionAmount = 100,
            },
            PromotionTypeEnum = PromotionTypeEnum.Cashback.Id,
            PromotionDescription = "promotion-description",
            PromotionStartDate = DateTime.MinValue,
            PromotionEndDate = DateTime.MaxValue,
            RewardRateTypeEnum = RewardRateTypeEnum.Fixed.Id,
            RateAmount = 123,
        };

        var validateCall = A.CallTo(() => _fakePromotionsValidationEngine.Validate(request));

        var createdPromotion = new Promotion();
        var createCall = A.CallTo(() => _fakePromotionsRepository.CreatePromotionAsync(A<Promotion>._, default));
        createCall.Invokes((Promotion x, CancellationToken _) => createdPromotion = x);
        createCall.ReturnsLazily(() => createdPromotion);

        // Act
        var response = await Service.CreatePromotionAsync(request, default);

        // Assert
        validateCall.MustHaveHappenedOnceExactly();
        createCall.MustHaveHappenedOnceExactly();

        response.Id.ShouldBe(createdPromotion.Id);

        createdPromotion.MerchantId.ShouldBe(request.MerchantId);
        createdPromotion.PromotionName.ShouldBe(request.PromotionName);
        createdPromotion.PromotionRules.Should().BeEquivalentTo(request.PromotionRules);
        createdPromotion.PromotionTypeEnum.ShouldBe(PromotionTypeEnum.Cashback);
        createdPromotion.PromotionDescription.ShouldBe(request.PromotionDescription);
        createdPromotion.PromotionStartDate.ShouldBe(request.PromotionStartDate);
        createdPromotion.PromotionEndDate.ShouldBe(request.PromotionEndDate);
        createdPromotion.RewardRateTypeEnum.ShouldBe(RewardRateTypeEnum.Fixed);
        createdPromotion.RateAmount.ShouldBe(request.RateAmount);
    }

    [Fact]
    [Trait("Class", "PromotionsService")]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetPromotionsFromQueryAsync")]
    [Description("Test the happy path for getting a promotion via query")]
    public async Task Test_Get_Promotions_From_Query_Happy_Path()
    {
        const string promotionOneId = "promotionOne";
        const string promotionTwoId = "promotionTwo";

        var promotionOne = new Promotion
        {
            Id = promotionOneId
        };

        var promotionTwo = new Promotion
        {
            Id = promotionTwoId
        };
        
        var repoCall = A.CallTo(() =>
            _fakePromotionsRepository.GetPromotionsFromQueryAsync(A<GetPromotionsQueryRequest>._, default));
        repoCall.Returns(new List<Promotion> { promotionOne, promotionTwo });

        var result = await Service.GetPromotionsFromQueryAsync(new GetPromotionsQueryRequest(), default);

        result.Count.ShouldBe(2);
        result.Any(x => x.Id == promotionOneId).ShouldBeTrue();
        result.Any(x => x.Id == promotionTwoId).ShouldBeTrue();
    }

}
