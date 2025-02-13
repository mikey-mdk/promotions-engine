using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using FakeItEasy;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using PromotionsEngine.Tests.Infrastructure.TestBase;

namespace PromotionsEngine.Tests.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class PromotionSummaryTests : IClassFixture<RepositoryTestBase>
{
    private readonly ILogger<PromotionSummaryRepository> _fakeLogger;
    private readonly RepositoryTestBase _testBase;

    private readonly PromotionSummaryRepository _repository;

    public PromotionSummaryTests(RepositoryTestBase testBase)
    {
        _testBase = testBase;
        _fakeLogger = A.Fake<ILogger<PromotionSummaryRepository>>();

        _repository = new PromotionSummaryRepository(testBase.FakeAzureClientFactory, testBase.FakeCosmosDbOptions, _fakeLogger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "GetPromotionSummaryAsync")]
    [Description("Test the happy path for getting a promotion summary")]
    public async Task Test_Get_Ledger_For_Order_Happy_Path()
    {
        const string promotionSummaryId = "promotionSummaryId";

        var promotionSummaryEntity = new PromotionSummaryEntity
        {
            Id = promotionSummaryId,
            TotalAmountRedeemed = 1000
        };

        var readItemConfiguration = _testBase.SetupReadItemAsyncSuccess(promotionSummaryEntity, promotionSummaryId);

        await _repository.GetPromotionSummaryAsync(promotionSummaryId, CancellationToken.None);

        readItemConfiguration.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "GetPromotionSummaryAsync")]
    [Description("Test the workflow for when the cosmos client returns a null response")]
    public async Task Test_Cosmos_Read_Item_Returns_Null()
    {
        const string promotionSummaryId = "promotionSummaryId";

        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        var containerConfig = A.CallTo(() =>
            _testBase.FakeCosmosContainer.ReadItemAsync<PromotionSummaryEntity>(A<string>._, A<PartitionKey>._,
                A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig!.Returns((ItemResponse<PromotionSummaryEntity>)null!);

        var result = await _repository.GetPromotionSummaryAsync(promotionSummaryId, CancellationToken.None);

        containerConfig.MustHaveHappenedOnceExactly();

        result?.Id.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "GetPromotionSummaryAsync")]
    [Description("Test the workflow for when the cosmos client throws not found")]
    public async Task Test_Cosmos_Read_Item_Throws_Not_Found_Exception()
    {
        const string promotionSummaryId = "promotionSummaryId";

        var notFoundConfig =
            _testBase.SetupReadItemAsyncNotFound<PromotionSummaryEntity>(promotionSummaryId);

        var result = await _repository.GetPromotionSummaryAsync(promotionSummaryId, CancellationToken.None);

        notFoundConfig.MustHaveHappenedOnceExactly();
        result?.Id.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "GetPromotionSummaryAsync")]
    [Description("Test the workflow for when the cosmos client throws unexpectedException")]
    public async Task Test_Cosmos_Read_Item_Throws_Generic_Exception()
    {
        const string promotionSummaryId = "promotionSummaryId";

        var notFoundConfig =
            _testBase.SetupReadItemAsyncGenericException<PromotionSummaryEntity>(promotionSummaryId);

        var result = await _repository.GetPromotionSummaryAsync(promotionSummaryId, CancellationToken.None);

        result.ShouldBeNull();
        notFoundConfig.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "UpdatePromotionSummaryAsync")]
    [Description("Test the happy path for updating a promotion summary")]
    public async Task Test_Cosmos_Upsert_Item_Happy_Path()
    {
        const string promotionSummaryId = "promotionSummaryId";

        var promotionSummaryEntity = new PromotionSummaryEntity
        {
            Id = promotionSummaryId,
            TotalAmountRedeemed = 1000
        };

        var upsertConfig = _testBase.SetupUpsertItemAsyncSuccess(promotionSummaryEntity);
        
        var result = await _repository.UpdatePromotionSummaryAsync(new PromotionSummary { Id = promotionSummaryId }, CancellationToken.None);

        result?.Id.ShouldBe(promotionSummaryId);
        upsertConfig.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "UpdatePromotionSummaryAsync")]
    [Description("Test the workflow when the cosmos client returns null")]
    public async Task Test_Cosmos_Client_Returns_Null()
    {
        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        var containerConfig = A.CallTo(_testBase.FakeCosmosContainer).Where(call => call.Method.Name == "UpsertItemAsync")
            .WithReturnType<Task<ItemResponse<PromotionSummaryEntity>>>();
        containerConfig.Returns((ItemResponse<PromotionSummaryEntity>)null!);

        var result = await _repository.UpdatePromotionSummaryAsync(new PromotionSummary(), CancellationToken.None);

        containerConfig.MustHaveHappenedOnceExactly();
        result?.Id.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryRepository))]
    [Trait("Method", "UpdatePromotionSummaryAsync")]
    [Description("Test the workflow when the cosmos client returns not found")]
    public async Task Test_Cosmos_Client_Throws_Generic_Exception()
    {
        var notFoundConfig = _testBase.SetupUpsertItemAsyncGenericException<PromotionSummaryEntity>();

        var result = await _repository.UpdatePromotionSummaryAsync(new PromotionSummary(), CancellationToken.None);

        result.ShouldBeNull();
        notFoundConfig.MustHaveHappenedOnceExactly();
    }
}