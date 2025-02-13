using FakeItEasy;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using PromotionsEngine.Tests.Infrastructure.TestBase;

namespace PromotionsEngine.Tests.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
public class CustomerOrderRewardsLedgerRepositoryTests : IClassFixture<RepositoryTestBase>
{
    private readonly ILogger<CustomerOrderRewardsLedgerRepository> _fakeLogger;
    private readonly RepositoryTestBase _testBase;

    private readonly CustomerOrderRewardsLedgerRepository _repository;

    public CustomerOrderRewardsLedgerRepositoryTests(RepositoryTestBase testBase)
    {
        _testBase = testBase;
        _fakeLogger = A.Fake<ILogger<CustomerOrderRewardsLedgerRepository>>();

        _repository = new CustomerOrderRewardsLedgerRepository(testBase.FakeAzureClientFactory, testBase.FakeCosmosDbOptions, _fakeLogger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", "CustomerOrderRewardsLedgerRepository")]
    [Trait("Method", "GetLedgerForOrder")]
    [Description("Test the happy path for getting a ledger for an order")]
    public async Task Test_Get_Ledger_For_Order_Happy_Path()
    {
        var documentId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();
        var orderId = Guid.NewGuid().ToString();
        var rewardBalance = 100m;

        var ledgerEntity = new CustomerOrderRewardsLedgerEntity
        {
            Id = documentId,
            CustomerId = customerId,
            OrderId = orderId,
            RewardBalance = rewardBalance,
            RewardTransactions = new List<RewardTransactionEntity>(),
            Merchant = new MerchantEntity(),
            Promotion = new PromotionEntity
            {
                PromotionRules = new PromotionRulesEntity
                {
                    NumberOfRedemptionsPerCustomer = 1
                }
            }
        };

        var expectedLedger = new CustomerOrderRewardsLedger
        {
            OrderId = orderId,
            CustomerId = customerId,
            Merchant = new Merchant(),
            Promotion = new Promotion(),
            RewardBalance = rewardBalance,
            RewardTransactions = new List<RewardTransaction>()
        };

        var readItemConfiguration = _testBase.SetupReadItemAsyncSuccess(ledgerEntity, orderId);

        var result = await _repository.GetLedgerForOrder(orderId, CancellationToken.None);

        readItemConfiguration.MustHaveHappenedOnceExactly();

        result.ShouldBeEquivalentTo(expectedLedger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", "CustomerOrderRewardsLedgerRepository")]
    [Trait("Method", "GetLedgerForOrder")]
    [Description("Test the workflow for when the cosmos client returns a null response")]
    public async Task Test_Cosmos_Read_Item_Returns_Null()
    {
        var orderId = Guid.NewGuid().ToString();

        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        var containerConfig = A.CallTo(() =>
            _testBase.FakeCosmosContainer.ReadItemAsync<CustomerOrderRewardsLedgerEntity>(A<string>._, A<PartitionKey>._,
                A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig!.Returns((ItemResponse<CustomerOrderRewardsLedgerEntity>)null!);

        var result = await _repository.GetLedgerForOrder(orderId, CancellationToken.None);

        containerConfig.MustHaveHappenedOnceExactly();

        result?.OrderId.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", "CustomerOrderRewardsLedgerRepository")]
    [Trait("Method", "GetLedgerForOrder")]
    [Description("Test the workflow for when the cosmos client returns a not found 404 exception")]
    public async Task Test_Cosmos_Read_Item_Throws_Not_Found_Exception()
    {
        var orderId = Guid.NewGuid().ToString();

        var notFoundConfig =
            _testBase.SetupReadItemAsyncNotFound<CustomerOrderRewardsLedgerEntity>(orderId);

        var result = await _repository.GetLedgerForOrder(orderId, CancellationToken.None);

        notFoundConfig.MustHaveHappenedOnceExactly();
        result?.OrderId.ShouldBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", "CustomerOrderRewardsLedgerRepository")]
    [Trait("Method", "GetLedgerForOrder")]
    [Description("Test the workflow for when the cosmos client throws an unexpected exception")]
    public async Task Test_Cosmos_Read_Item_Throws_Generic_Exception()
    {
        var orderId = Guid.NewGuid().ToString();

        var notFoundConfig =
            _testBase.SetupReadItemAsyncGenericException<CustomerOrderRewardsLedgerEntity>(orderId);

        var result = await _repository.GetLedgerForOrder(orderId, CancellationToken.None);

        result.ShouldBeNull();
        notFoundConfig.MustHaveHappenedOnceExactly();
    }
}