using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using PromotionsEngine.Application.Cache.Implementations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;

namespace PromotionsEngine.Tests.Application.Cache;

[ExcludeFromCodeCoverage]
public class RedisCacheManagerTests
{
    private readonly IConnectionMultiplexer _fakeConnectionMultiplexer;
    private readonly IMerchantRepository _fakeMerchantRepository;
    private readonly IPromotionsRepository _fakePromotionsRepository;
    private readonly ILogger<RedisCacheManager> _fakeLogger;

    private readonly IDatabase _fakeDatabase;

    private readonly RedisCacheManager _redisCacheManager;

    public RedisCacheManagerTests()
    {
        _fakeConnectionMultiplexer = A.Fake<IConnectionMultiplexer>();
        _fakeLogger = A.Fake<ILogger<RedisCacheManager>>();

        _fakeMerchantRepository = A.Fake<IMerchantRepository>();
        _fakePromotionsRepository = A.Fake<IPromotionsRepository>();
        _fakeDatabase = A.Fake<IDatabase>();

        _redisCacheManager = new RedisCacheManager(_fakeLogger, _fakeConnectionMultiplexer);
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetOrSetAsync")]
    [Description("Test the happy path for when cache is empty")]
    public async Task Test_Happy_Path_For_Checkout_Empty_Cache()
    {
        var merchantId = Guid.NewGuid().ToString();
        var promotionOneId = Guid.NewGuid().ToString();
        var promotionTwoId = Guid.NewGuid().ToString();

        var testMerchant = new Merchant
        {
            MerchantId = merchantId
        };

        var promotionOne = new Promotion
        {
            MerchantId = merchantId,
            Id = promotionOneId
        };

        var promotionTwo = new Promotion
        {
            MerchantId = merchantId,
            Id = promotionTwoId
        };

        var funcToExecute = async () =>
        {
            var merchant = await _fakeMerchantRepository.GetMerchantByIdAsync(merchantId, CancellationToken.None);

            if (merchant == null)
            {
                _fakeLogger.LogError(
                    "Unable to find merchant for GetOffersForCheckoutWorkflow with merchantId: {merchantId}",
                    merchantId);
                return (new Merchant(), new List<Promotion>());
            }

            var promotions =
                await _fakePromotionsRepository.GetPromotionsByMerchantIdAsync(merchant.MerchantId,
                    CancellationToken.None);

            return (merchant, promotions);
        };

        var merchantCall = A.CallTo(() =>
            _fakeMerchantRepository.GetMerchantByIdAsync(A<string>._, A<CancellationToken>._));
        merchantCall.Returns(testMerchant);

        var promotionsCall = A.CallTo(() =>
            _fakePromotionsRepository.GetPromotionsByMerchantIdAsync(A<string>._, A<CancellationToken>._));
        promotionsCall.Returns(new List<Promotion> { promotionOne, promotionTwo });

        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        var getDatabaseCall = A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._));
        getDatabaseCall.Returns(_fakeDatabase);

        var getAsyncCall = A.CallTo(() =>
            _fakeDatabase.StringGetAsync(A<RedisKey>.That.Matches(x => x == merchantId), A<CommandFlags>._));
        getAsyncCall.Returns(new RedisValue());

        var setAsyncCall = A.CallTo(() => _fakeDatabase.StringSetAsync(A<RedisKey>._, A<RedisValue>._, A<TimeSpan>._,
            A<bool>._, A<When>._, A<CommandFlags>._));
        setAsyncCall.Returns(true);

        var response = await _redisCacheManager.GetOrSetAsync(merchantId, funcToExecute);

        response.Item1.MerchantId.ShouldBe(merchantId);
        response.Item2.Count.ShouldBe(2);
        response.Item2.ShouldContain(x => x.Id == promotionOneId);
        response.Item2.ShouldContain(x => x.Id == promotionTwoId);

        merchantCall.MustHaveHappenedOnceExactly();
        promotionsCall.MustHaveHappenedOnceExactly();

        getDatabaseCall.MustHaveHappenedTwiceExactly();
        getAsyncCall.MustHaveHappenedOnceExactly();
        setAsyncCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetOrSetAsync")]
    [Description("Test the happy path for when cache is empty")]
    public async Task Test_Happy_Path_For_Checkout_Cache_Exists()
    {
        var merchantId = Guid.NewGuid().ToString();
        var promotionOneId = Guid.NewGuid().ToString();
        var promotionTwoId = Guid.NewGuid().ToString();

        var testMerchant = new Merchant
        {
            MerchantId = merchantId
        };

        var promotionOne = new Promotion
        {
            MerchantId = merchantId,
            Id = promotionOneId
        };

        var promotionTwo = new Promotion
        {
            MerchantId = merchantId,
            Id = promotionTwoId
        };

        var cachedValue = ValueTuple.Create(testMerchant, new List<Promotion> { promotionOne, promotionTwo });

        var jsonCachedValue = JsonSerializer.Serialize(cachedValue, new JsonSerializerOptions { IncludeFields = true });

        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        var getDatabaseCall = A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._));
        getDatabaseCall.Returns(_fakeDatabase);

        var getAsyncCall = A.CallTo(() =>
            _fakeDatabase.StringGetAsync(A<RedisKey>.That.Matches(x => x == merchantId), A<CommandFlags>._));
        getAsyncCall.Returns(new RedisValue(jsonCachedValue));

        var func = async () =>
        {
            var foobar = await _fakeMerchantRepository.GetMerchantByIdAsync(A<string>._, A<CancellationToken>._);
            return ValueTuple.Create(new Merchant(), new List<Promotion> { new() });
        };

        var response = await _redisCacheManager.GetOrSetAsync(merchantId, func);

        response.Item1.MerchantId.ShouldBe(merchantId);
        response.Item2.Count.ShouldBe(2);
        response.Item2.ShouldContain(x => x.Id == promotionOneId);
        response.Item2.ShouldContain(x => x.Id == promotionTwoId);

        getDatabaseCall.MustHaveHappenedOnceExactly();
        getAsyncCall.MustHaveHappenedOnceExactly();

        A.CallTo(() => _fakeDatabase.StringSetAsync(A<RedisKey>._, A<RedisValue>._, A<TimeSpan>._,
            A<bool>._, A<When>._, A<CommandFlags>._)).MustNotHaveHappened();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetOrSetAsync")]
    [Description("Test empty result is returned when cache is empty and func returns null or empty")]
    public async Task Test_Cache_Empty_Data_Null_Or_Empty()
    {
        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        var getDatabaseCall = A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._));
        getDatabaseCall.Returns(_fakeDatabase);

        var getAsyncCall = A.CallTo(() =>
            _fakeDatabase.StringGetAsync(A<RedisKey>._, A<CommandFlags>._));
        getAsyncCall.Returns(new RedisValue());

        var func = async () =>
        {
            await _fakeMerchantRepository.GetMerchantByIdAsync("foo", CancellationToken.None);
            return new ValueTuple<Merchant, List<Promotion>>();
        };

        var response = await _redisCacheManager.GetOrSetAsync("test", func);

        response.Item1.Should().BeNull();
        response.Item2.Should().BeNull();

        A.CallTo(() => _fakeDatabase.StringSetAsync(A<RedisKey>._, A<RedisValue>._, A<TimeSpan>._,
            A<bool>._, A<When>._, A<CommandFlags>._)).MustNotHaveHappened();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetOrSetAsync")]
    [Description("Test attempt to set cache returns false")]
    public async Task Test_Set_Cache_Attempt_Returns_False()
    {
        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        var getDatabaseCall = A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._));
        getDatabaseCall.Returns(_fakeDatabase);

        var getAsyncCall = A.CallTo(() =>
            _fakeDatabase.StringGetAsync(A<RedisKey>._, A<CommandFlags>._));
        getAsyncCall.Returns(new RedisValue());

        var setAsyncCall = A.CallTo(() => _fakeDatabase.StringSetAsync(A<RedisKey>._, A<RedisValue>._, A<TimeSpan>._,
            A<bool>._, A<When>._, A<CommandFlags>._));
        setAsyncCall.Returns(false);

        var func = async () =>
        {
            await _fakeMerchantRepository.GetMerchantByIdAsync("foo", CancellationToken.None);
            return ValueTuple.Create(new Merchant(), new List<Promotion> { new() });
        };

        await _redisCacheManager.GetOrSetAsync("test", func);

        getDatabaseCall.MustHaveHappenedTwiceExactly();
        getAsyncCall.MustHaveHappenedOnceExactly();
        setAsyncCall.MustHaveHappenedOnceExactly();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetOrSetAsync")]
    [Description("Test multiplexer is not connected")]
    public async Task Test_Multiplexer_Not_Connected()
    {
        var merchantId = "merchantId";

        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(false);

        var func = async () =>
        {
            await _fakeMerchantRepository.GetMerchantByIdAsync("foo", CancellationToken.None);
            return ValueTuple.Create(new Merchant { MerchantId = merchantId },
                new List<Promotion> { new() { MerchantId = merchantId } });
        };

        var response = await _redisCacheManager.GetOrSetAsync("test", func);

        response.Item1.MerchantId.ShouldBe(merchantId);
        response.Item2.First().MerchantId.ShouldBe(merchantId);

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceOrMore();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "GetOrSetAsync")]
    [Description("Test multiplexer throws exception")]
    public async Task Test_Multiplexer_Throws_Exception()
    {
        var merchantId = "merchantId";

        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Throws(new Exception("something bad happened"));

        var func = async () =>
        {
            await _fakeMerchantRepository.GetMerchantByIdAsync("foo", CancellationToken.None);
            return ValueTuple.Create(new Merchant { MerchantId = merchantId },
                new List<Promotion> { new() { MerchantId = merchantId } });
        };

        var response = await _redisCacheManager.GetOrSetAsync("test", func);

        response.Item1.MerchantId.ShouldBe(merchantId);
        response.Item2.First().MerchantId.ShouldBe(merchantId);

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceOrMore();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "InvalidateCache")]
    [Description("Test invalidate success")]
    public async Task Test_Cache_Invalidate_Success()
    {
        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        var getDatabaseCall = A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._));
        getDatabaseCall.Returns(_fakeDatabase);

        var keyDeleteCall = A.CallTo(() => _fakeDatabase.KeyDeleteAsync(A<RedisKey>._, A<CommandFlags>._));
        keyDeleteCall.Returns(true);

        await _redisCacheManager.InvalidateCache("test");

        keyDeleteCall.MustHaveHappenedOnceExactly();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustNotHaveHappened();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "InvalidateCache")]
    [Description("Test invalidate returns null when multiplexer is unavailable")]
    public async Task Test_Cache_Invalidate_Fails_When_Multiplexer_Is_Unavailable()
    {
        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(false);

        await _redisCacheManager.InvalidateCache("test");

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustNotHaveHappened();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "InvalidateCache")]
    [Description("Test invalidate failure")]
    public async Task Test_Cache_Invalidate_Failure()
    {
        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        var getDatabaseCall = A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._));
        getDatabaseCall.Returns(_fakeDatabase);

        var keyDeleteCall = A.CallTo(() => _fakeDatabase.KeyDeleteAsync(A<RedisKey>._, A<CommandFlags>._));
        keyDeleteCall.Returns(false);

        await _redisCacheManager.InvalidateCache("test");

        keyDeleteCall.MustHaveHappenedOnceExactly();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Class", nameof(RedisCacheManager))]
    [Trait("Category", "Unit")]
    [Trait("Method", "InvalidateCache")]
    [Description("Test invalidate throws exception")]
    public async Task Test_Cache_Invalidate_Throws_Exception()
    {
        A.CallTo(() => _fakeConnectionMultiplexer.IsConnected).Returns(true);

        A.CallTo(() => _fakeConnectionMultiplexer.GetDatabase(A<int>._, A<object?>._)).Throws(new Exception("something bad happened"));

        await _redisCacheManager.InvalidateCache("test");

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceOrMore();
    }
}