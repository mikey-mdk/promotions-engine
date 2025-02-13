using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Cache;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Application.Engines.Interfaces;
using PromotionsEngine.Application.Services.Implementations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Application.Services;

[ExcludeFromCodeCoverage]
public class MerchantIdentificationServiceTests
{
    private readonly IMerchantRepository _fakeMerchantRepository;
    private readonly IRedisCacheManager _fakeRedisCacheManager;
    private readonly IMerchantRegexRepository _fakeMerchantRegexRepository;
    private readonly IRegexEvaluationEngine _fakeRegexEvaluationEngine;
    private readonly ILogger<MerchantIdentificationService> _fakeLogger;

    private readonly IFixture _fixture;

    private readonly MerchantIdentificationService _merchantIdentificationService;

    public MerchantIdentificationServiceTests()
    {
        _fakeMerchantRepository = A.Fake<IMerchantRepository>();
        _fakeRedisCacheManager = A.Fake<IRedisCacheManager>();
        _fakeMerchantRegexRepository = A.Fake<IMerchantRegexRepository>();
        _fakeRegexEvaluationEngine = A.Fake<IRegexEvaluationEngine>();
        _fakeLogger = A.Fake<ILogger<MerchantIdentificationService>>();

        _fixture = new Fixture();

        _merchantIdentificationService = new MerchantIdentificationService(
            _fakeMerchantRepository,
            _fakeRedisCacheManager,
            _fakeMerchantRegexRepository,
            _fakeRegexEvaluationEngine,
            _fakeLogger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantIdentificationService))]
    [Trait("Method", nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync))]
    [Description($"Test happy path for {nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync)}")]
    public async Task Test_Identify_Merchant_By_Regex_Success()
    {
        var fakeMerchantRegexOne = A.Fake<MerchantRegex>(x => _fixture.Create<MerchantRegex>());
        var fakeMerchantRegexTwo = A.Fake<MerchantRegex>(x => _fixture.Create<MerchantRegex>());

        var merchantRegexList = new List<MerchantRegex>
        {
            fakeMerchantRegexOne,
            fakeMerchantRegexTwo
        };

        var redisGetCall = A.CallTo(() =>
            _fakeRedisCacheManager.GetOrSetAsync(
                A<string>.That.Matches(x => x == CRedisCacheKeys.MerchantRegexLookupCacheKey),
                A<Func<Task<List<MerchantRegex>>>>._));
        redisGetCall.Returns(merchantRegexList);

        var regexCall = A.CallTo(() => _fakeRegexEvaluationEngine.EvaluateRegexList(A<string>._, A<List<string>>.That.IsEqualTo(fakeMerchantRegexOne.RegexPatterns)));
        regexCall.Returns(new List<string> { fakeMerchantRegexOne.RegexPatterns.FirstOrDefault()! });

        await _merchantIdentificationService.IdentifyMerchantByRegexAsync("merchantName", default);

        redisGetCall.MustHaveHappenedOnceExactly();
        regexCall.MustHaveHappenedOnceExactly();

        A.CallTo(() =>
            _fakeMerchantRepository.GetMerchantByIdAsync(A<string>.That.Matches(x => x == fakeMerchantRegexOne.Id),
                A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantIdentificationService))]
    [Trait("Method", nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync))]
    [Description(
        $"Test empty {nameof(MerchantRegex)} list for {nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync)}")]
    public async Task Test_Empty_Merchant_Regex_List_Returns_Null()
    {
        A.CallTo(() => _fakeRedisCacheManager.GetOrSetAsync(A<string>._, A<Func<Task<List<MerchantRegex>>>>._))
            .Returns(new List<MerchantRegex>());

        var response = await _merchantIdentificationService.IdentifyMerchantByRegexAsync("merchantName", default);

        response.ShouldBeNull();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantIdentificationService))]
    [Trait("Method", nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync))]
    [Description(
        $"Test no regex matches for {nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync)}")]
    public async Task Test_No_Regex_Matches_Returns_Null()
    {
        var fakeMerchantRegexOne = A.Fake<MerchantRegex>(x => _fixture.Create<MerchantRegex>());
        var fakeMerchantRegexTwo = A.Fake<MerchantRegex>(x => _fixture.Create<MerchantRegex>());

        var merchantRegexList = new List<MerchantRegex>
        {
            fakeMerchantRegexOne,
            fakeMerchantRegexTwo
        };

        var redisGetCall = A.CallTo(() =>
            _fakeRedisCacheManager.GetOrSetAsync(
                A<string>.That.Matches(x => x == CRedisCacheKeys.MerchantRegexLookupCacheKey),
                A<Func<Task<List<MerchantRegex>>>>._));
        redisGetCall.Returns(merchantRegexList);

        var regexCall = A.CallTo(() => _fakeRegexEvaluationEngine.EvaluateRegexList(A<string>._, A<List<string>>._));
        regexCall.Returns(new List<string>());

        var response = await _merchantIdentificationService.IdentifyMerchantByRegexAsync("merchantName", default);

        response.ShouldBeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantIdentificationService))]
    [Trait("Method", nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync))]
    [Description($"Test exception encountered in {nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync)}")]
    public async Task Test_Exception_Encountered_Returns_Null()
    {
        A.CallTo(() => _fakeRedisCacheManager.GetOrSetAsync(A<string>._, A<Func<Task<List<MerchantRegex>>>>._))
            .Throws(new Exception("whoops"));

        var response = await _merchantIdentificationService.IdentifyMerchantByRegexAsync("merchantName", default);

        response.ShouldBeNull();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantIdentificationService))]
    [Trait("Method", nameof(MerchantIdentificationService.IdentifyMerchantByRegexAsync))]
    [Description($"Test empty merchant name")]
    public async Task Test_Empty_Merchant_Name_Returns_Null()
    {
        var response = await _merchantIdentificationService.IdentifyMerchantByRegexAsync(string.Empty, default);

        response.ShouldBeNull();
    }
}