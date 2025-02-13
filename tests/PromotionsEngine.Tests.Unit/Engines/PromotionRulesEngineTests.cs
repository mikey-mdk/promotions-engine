using System.Diagnostics.CodeAnalysis;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Xunit;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Engines.RulesEngines.Implementations;
using PromotionsEngine.Domain.Engines.RulesEngines.Interfaces;
using PromotionsEngine.Domain.Engines.RulesEngines.Requests;
using PromotionsEngine.Domain.Helpers.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Domain.Engines;

[ExcludeFromCodeCoverage]
public class PromotionRulesEngineTests
{
    private readonly IPromotionSummaryRepository _fakePromotionSummaryRepository;
    private readonly IPromotionRulesEngineHelper _fakePromotionRulesEngineHelper;
    private readonly ILogger<PromotionRulesEngine> _fakeLogger;

    private readonly IPromotionRulesEngine _engine;

    public PromotionRulesEngineTests()
    {
        _fakePromotionSummaryRepository = A.Fake<IPromotionSummaryRepository>();
        _fakePromotionRulesEngineHelper = A.Fake<IPromotionRulesEngineHelper>();
        _fakeLogger = A.Fake<ILogger<PromotionRulesEngine>>();

        _engine = new PromotionRulesEngine(_fakePromotionSummaryRepository, _fakePromotionRulesEngineHelper,
            _fakeLogger);
    }

    public static TheoryData<string> DefaultData = new()
    {
        CPromotionRuleEvaluationContext.OrderCreated,
        CPromotionRuleEvaluationContext.CheckoutPresentation,
        CPromotionRuleEvaluationContext.AppPresentation,
        CPromotionRuleEvaluationContext.OrderRefunded,
        "nonsense"
    };

    public static TheoryData<string> SummaryData = new()
    {
        CPromotionRuleEvaluationContext.OrderRefunded,
        CPromotionRuleEvaluationContext.OrderCreated,
        CPromotionRuleEvaluationContext.CheckoutPresentation,
        CPromotionRuleEvaluationContext.AppPresentation,
        "nonsense"
    };
    
    [Theory]
    [MemberData(nameof(DefaultData))]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", "PromotionRulesEngine")]
    [Trait("Method", "EvaluateDefaultPromotionRules")]
    [Description("Test that the various rules contexts call the correct helper method for default rules")]
    public async Task Test_Evaluate_Default_Promotion_Rules(string evaluationContext)
    {
        await _engine.EvaluateDefaultPromotionRules(new Promotion(), 100m, evaluationContext);

        if (!CPromotionRuleEvaluationContext.GetAll().Contains(evaluationContext))
        {
            A.CallTo(_fakeLogger)
                .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
                .MustHaveHappenedOnceExactly();
            
        }
        else if (evaluationContext.Equals(CPromotionRuleEvaluationContext.OrderRefunded))
        {
            A.CallTo(() => _fakePromotionRulesEngineHelper.EvaluateAllDefaultRules(A<Promotion>._, A<decimal>._, null))
                .MustNotHaveHappened();
            A.CallTo(() =>
                    _fakePromotionRulesEngineHelper.EvaluateDefaultRulesForRefundContext(A<Promotion>._, A<decimal>._))
                .MustHaveHappenedOnceExactly();
        }
        else
        {
            A.CallTo(() => _fakePromotionRulesEngineHelper.EvaluateDefaultRulesForRefundContext(A<Promotion>._, A<decimal>._))
                .MustNotHaveHappened();
            A.CallTo(() =>
                    _fakePromotionRulesEngineHelper.EvaluateAllDefaultRules(A<Promotion>._, A<decimal>._, null))
                .MustHaveHappenedOnceExactly();
        }
    }

    [Theory]
    [MemberData(nameof(SummaryData))]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", "PromotionRulesEngine")]
    [Trait("Method", "EvaluatePromotionSummaryRules")]
    [Description("Test that the various rules contexts call the correct helper method for summary rules")]
    public async Task Test_Evaluate_Promotion_Summary_Rules(string evaluationContext)
    {
        var result =
            await _engine.EvaluatePromotionSummaryRules(new Promotion(), new PromotionSummary(), 100m,
                evaluationContext);

        if (evaluationContext.Equals(CPromotionRuleEvaluationContext.OrderRefunded))
        {
            //We never want to evaluate summary rules for refunds.
            result.ShouldBeTrue();
        }
        else if (CPromotionRuleEvaluationContext.GetAll().Contains(evaluationContext))
        {
            A.CallTo(() =>
                    _fakePromotionRulesEngineHelper.EvaluateAllSummaryRules(A<Promotion>._, A<PromotionSummary>._))
                .MustHaveHappenedOnceExactly();
        }
        else
        {
            A.CallTo(_fakeLogger)
                .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
                .MustHaveHappenedOnceExactly();
        }
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Engine")]
    [Trait("Class", "PromotionRulesEngine")]
    [Trait("Method", "FindValidPromotions")]
    [Description("Test that the various rules contexts call the correct helper method for default rules")]
    public async Task Test_Find_Valid_Promotions_Order_Created_Context()
    {
        //One promotion with invalid default rules
        //One promotion with invalid summary rules
        //one valid promotion

        const decimal orderAmount = 100m;

        const string promotionOneId = "promotionOne";
        const string promotionTwoId = "promotionTwo";
        const string promotionThreeId = "promotionThree";

        //fails default evaluation
        var promotionOne = new Promotion
        {
            Id = promotionOneId,
        };

        //passes default evaluation, fails summary evaluation
        var promotionTwo = new Promotion
        {
            Id = promotionTwoId,
        };

        //passes both default and summary evaluation
        var promotionThree = new Promotion
        {
            Id = promotionThreeId,
        };

        var promotionSummaryTwo = new PromotionSummary
        {
            Id = promotionTwoId
        };

        var promotionSummaryThree = new PromotionSummary
        {
            Id = promotionThreeId
        };

        var request = new FindValidPromotionsRequest
        {
            Promotions = new List<Promotion>
            {
                promotionOne,
                promotionTwo,
                promotionThree
            },
            OrderAmount = orderAmount,
            EvaluationContext = CPromotionRuleEvaluationContext.OrderCreated
        };

        var promotionOneDefaultHelperCall = A.CallTo(() =>
            _fakePromotionRulesEngineHelper.EvaluateAllDefaultRules(
                A<Promotion>.That.Matches(x => x.Id == promotionOneId), orderAmount, null));
        promotionOneDefaultHelperCall.Returns(false);

        var promotionTwoDefaultHelperCall = A.CallTo(() =>
            _fakePromotionRulesEngineHelper.EvaluateAllDefaultRules(
                A<Promotion>.That.Matches(x => x.Id == promotionTwoId), orderAmount, null));
        promotionTwoDefaultHelperCall.Returns(true);

        var promotionThreeDefaultHelperCall = A.CallTo(() =>
            _fakePromotionRulesEngineHelper.EvaluateAllDefaultRules(
                A<Promotion>.That.Matches(x => x.Id == promotionThreeId), orderAmount, null));
        promotionThreeDefaultHelperCall.Returns(true);

        var promotionTwoRepositorySummaryHelperCall = A.CallTo(() =>
            _fakePromotionSummaryRepository.GetPromotionSummaryAsync(A<string>.That.Matches(x => x == promotionTwoId),
                CancellationToken.None));
        promotionTwoRepositorySummaryHelperCall.Returns(promotionSummaryTwo);

        var promotionTwoHelperSummaryCall = A.CallTo(() =>
            _fakePromotionRulesEngineHelper.EvaluateAllSummaryRules(
                A<Promotion>.That.Matches(x => x.Id == promotionTwoId),
                A<PromotionSummary>.That.Matches(x => x.Id == promotionTwoId)));
        promotionTwoHelperSummaryCall.Returns(false);

        var promotionThreeRepositorySummaryHelperCall = A.CallTo(() =>
            _fakePromotionSummaryRepository.GetPromotionSummaryAsync(A<string>.That.Matches(x => x == promotionThreeId),
                CancellationToken.None));
        promotionThreeRepositorySummaryHelperCall.Returns(promotionSummaryThree);

        var promotionThreeHelperSummaryCall = A.CallTo(() =>
            _fakePromotionRulesEngineHelper.EvaluateAllSummaryRules(
                A<Promotion>.That.Matches(x => x.Id == promotionThreeId),
                A<PromotionSummary>.That.Matches(x => x.Id == promotionThreeId)));
        promotionThreeHelperSummaryCall.Returns(true);

        var result = await _engine.FindValidPromotions(request);

        result.Count.ShouldBe(1);
        result.First().promotion.Id.ShouldBe(promotionThreeId);
        result.First().promotionSummary.Id.ShouldBe(promotionThreeId);

        promotionOneDefaultHelperCall.MustHaveHappenedOnceExactly();
        promotionTwoDefaultHelperCall.MustHaveHappenedOnceExactly();
        promotionThreeDefaultHelperCall.MustHaveHappenedOnceExactly();

        promotionTwoRepositorySummaryHelperCall.MustHaveHappenedOnceExactly();
        promotionTwoHelperSummaryCall.MustHaveHappenedOnceExactly();

        promotionThreeRepositorySummaryHelperCall.MustHaveHappenedOnceExactly();
        promotionThreeHelperSummaryCall.MustHaveHappenedOnceExactly();
    }

    //use theory for default promotion rules

    //use theory for summary promotion rules
}