using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Cache.Interfaces;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Infrastructure.ChangeFeed.Implementations;
using PromotionsEngine.Tests.Infrastructure.TestBase;

namespace PromotionsEngine.Tests.Infrastructure.ChangeFeed;

/// <summary>
/// The Cosmos Change Feed library is currently preventing proper unit testing patterns do to internal scoped fields that are not accessible and unable to be mocked.
/// Specifically, the ChangeFeedProcessorBuildClass.WithLeaseInternal(leaseContainer) method blows up when trying to mock the leaseContainer.
/// We could create abstractions to assist with test coverage but I don't think we have the time before MVP to fall down that rabbit hole. We will need to return to this when we have time.
/// </summary>
[ExcludeFromCodeCoverage]
public class CosmosChangeFeedProcessorTests : IClassFixture<RepositoryTestBase>
{
    private readonly IRedisCacheManager _fakeRedisCacheManager;
    private readonly ILogger<CosmosChangeFeedProcessor> _fakeLogger;
    private readonly IMerchantRegexRepository _fakeMerchantRegexRepository;
    private readonly RepositoryTestBase _testBase;

#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly CosmosChangeFeedProcessor _changeFeedProcessor;
#pragma warning restore S4487 // Unread "private" fields should be removed

    public CosmosChangeFeedProcessorTests(RepositoryTestBase testBase)
    {
        _testBase = testBase;

        _fakeRedisCacheManager = A.Fake<IRedisCacheManager>();
        _fakeLogger = A.Fake<ILogger<CosmosChangeFeedProcessor>>();
        _fakeMerchantRegexRepository = A.Fake<IMerchantRegexRepository>();

        _changeFeedProcessor = new CosmosChangeFeedProcessor(_testBase.FakeAzureClientFactory,
            _testBase.FakeCosmosDbOptions, _fakeRedisCacheManager, _fakeLogger, _fakeMerchantRegexRepository);
    }
}
