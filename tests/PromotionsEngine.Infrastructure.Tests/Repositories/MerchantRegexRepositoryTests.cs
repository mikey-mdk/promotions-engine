using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using PromotionsEngine.Tests.Infrastructure.TestBase;

namespace PromotionsEngine.Tests.Infrastructure.Repositories;

public class MerchantRegexRepositoryTests : IClassFixture<RepositoryTestBase>
{
    private readonly ILogger<MerchantRegexRepository> _fakeLogger;
    private readonly RepositoryTestBase _testBase;
    private readonly Fixture _fixture;

    private readonly MerchantRegexRepository _repository;

    public MerchantRegexRepositoryTests(RepositoryTestBase testBase)
    {
        _testBase = testBase;
        _fakeLogger = A.Fake<ILogger<MerchantRegexRepository>>();

        _fixture = new Fixture();

        _repository = new MerchantRegexRepository(testBase.FakeAzureClientFactory, testBase.FakeCosmosDbOptions, _fakeLogger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRegexRepository))]
    [Trait("Method", nameof(MerchantRegexRepository.CreateMerchantRegexAsync))]
    [Description(
        $"Test {nameof(MerchantRegexRepository.CreateMerchantRegexAsync)} returns a {nameof(MerchantRegex)} when a document is created")]
    public async Task Test_Create_Merchant_Regex_Happy_Path()
    {
        var merchantRegex = _fixture.Create<MerchantRegex>();

        var merchantRegexEntity = merchantRegex.MapToEntity();

        var readItemConfiguration = _testBase.SetupCreateItemAsyncSuccess(merchantRegexEntity);

        var result = await _repository.CreateMerchantRegexAsync(merchantRegex, CancellationToken.None);

        result.Should().BeEquivalentTo(merchantRegex);
        readItemConfiguration.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRegexRepository))]
    [Trait("Method", nameof(MerchantRegexRepository.CreateMerchantRegexAsync))]
    [Description(
        $"Test {nameof(MerchantRegexRepository.CreateMerchantRegexAsync)} returns a null when an exception occurs")]
    public async Task Test_Create_Merchant_Regex_Null_Response()
    {
        var merchantRegex = _fixture.Create<MerchantRegex>();

        var createConfig = A.CallTo(_testBase.FakeCosmosContainer).Where(call => call.Method.Name == "CreateItemAsync")
            .WithReturnType<Task<ItemResponse<MerchantRegexEntity>>>();
        createConfig.Returns((ItemResponse<MerchantRegexEntity>)null!);

        var result = await _repository.CreateMerchantRegexAsync(merchantRegex, CancellationToken.None);

        result.ShouldBeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRegexRepository))]
    [Trait("Method", nameof(MerchantRegexRepository.ReplaceRegexPatternsAsync))]
    [Description(
        $"Test {nameof(MerchantRegexRepository.ReplaceRegexPatternsAsync)} returns a {nameof(MerchantRegex)} when a document patched successfully")]
    public async Task Test_Replace_Regex_Patterns_Happy_Path()
    {
        var merchantRegex = _fixture.Create<MerchantRegex>();
        var merchantRegexEntity = merchantRegex.MapToEntity();

        var fakeItemResponse = A.Fake<ItemResponse<MerchantRegexEntity>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(merchantRegexEntity);

        var patchCall = A.CallTo(() =>
            _testBase.FakeCosmosContainer.PatchItemAsync<MerchantRegexEntity>(A<string>.That.Matches(x => x == merchantRegex.Id),
                A<PartitionKey>._, A<List<PatchOperation>>._, A<PatchItemRequestOptions>._, CancellationToken.None));
        patchCall.Returns(fakeItemResponse);

        var response = await _repository.ReplaceRegexPatternsAsync(merchantRegex, CancellationToken.None);

        patchCall.MustHaveHappenedOnceExactly();
        response.Should().BeEquivalentTo(merchantRegex);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRegexRepository))]
    [Trait("Method", nameof(MerchantRegexRepository.ReplaceRegexPatternsAsync))]
    [Description(
        $"Test {nameof(MerchantRegexRepository.ReplaceRegexPatternsAsync)} calls {nameof(MerchantRegexRepository.CreateMerchantRegexAsync)} when document not found")]
    public async Task Test_Replace_Regex_Patterns_Calls_Create_When_Document_Not_Found()
    {
        var merchantRegex = _fixture.Create<MerchantRegex>();
        var merchantRegexEntity = merchantRegex.MapToEntity();

        A.CallTo(() =>
            _testBase.FakeCosmosContainer.PatchItemAsync<MerchantRegexEntity>(A<string>.That.Matches(x => x == merchantRegex.Id),
                A<PartitionKey>._, A<List<PatchOperation>>._, A<PatchItemRequestOptions>._, CancellationToken.None)).Throws(new CosmosException("blah", HttpStatusCode.NotFound, 0, "", 0));


        var readItemConfiguration = _testBase.SetupCreateItemAsyncSuccess(merchantRegexEntity);

        await _repository.ReplaceRegexPatternsAsync(merchantRegex, CancellationToken.None);

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();

        readItemConfiguration.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRegexRepository))]
    [Trait("Method", nameof(MerchantRegexRepository.ReplaceRegexPatternsAsync))]
    [Description(
        $"Test {nameof(MerchantRegexRepository.ReplaceRegexPatternsAsync)} returns null when exception occurs")]
    public async Task Test_Replace_Regex_Patterns_Returns_Null_When_Exception_Occurs()
    {
        var merchantRegex = _fixture.Create<MerchantRegex>();

        A.CallTo(() =>
                _testBase.FakeCosmosContainer.PatchItemAsync<MerchantRegexEntity>(
                    A<string>.That.Matches(x => x == merchantRegex.Id),
                    A<PartitionKey>._, A<List<PatchOperation>>._, A<PatchItemRequestOptions>._, CancellationToken.None))
            .Throws(new Exception("whoops"));

        var response = await _repository.ReplaceRegexPatternsAsync(merchantRegex, CancellationToken.None);

        response.ShouldBeNull();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();
    }
}