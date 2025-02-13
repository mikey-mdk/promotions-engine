using FakeItEasy;
using FakeItEasy.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using PromotionsEngine.Infrastructure.Configuration;
using PromotionsEngine.Tests.Infrastructure.Repositories;

namespace PromotionsEngine.Tests.Infrastructure.TestBase;

[ExcludeFromCodeCoverage]
public class RepositoryTestBase : IDisposable
{
    /// <summary>
    /// This class is intended to be used to bootstrap the default setup for all repository dependencies around Cosmos.
    /// The various Cosmos items are individual public properties in case you need to override them in a test.
    /// <see cref="SampleRepositoryTestClass"/> for most basic usage.
    /// </summary>
    public RepositoryTestBase()
    {
        FakeAzureClientFactory = A.Fake<IAzureClientFactory<CosmosClient>>();
        FakeCosmosDbOptions = Options.Create(new CosmosDbOptions
        {
            CustomerOrderRewardsLedgerContainerName = "testqueue",
            DatabaseName = "testdbname",
            MerchantContainerName = "testmerchantcontainer",
            MerchantLeaseContainerName = "testMerchantLeaseContainer",
            PromotionsContainerName = "testpromotionscontainer",
            PromotionsLeaseContainerName = "testPromotionLeaseContainer",
            PromotionSummaryContainerName = "testpromotionssummarycontainer"
        });

        FakeCosmosClient = A.Fake<CosmosClient>();
        FakeCosmosDatabase = A.Fake<Database>();
        FakeCosmosContainer = A.Fake<Container>();

        //This setup can likely be placed into a repository test fixture.
        A.CallTo(() => FakeAzureClientFactory.CreateClient(A<string>._))!.Returns(FakeCosmosClient);
        A.CallTo(() => FakeCosmosClient!.GetDatabase(A<string>._))!.Returns(FakeCosmosDatabase);
        A.CallTo(() => FakeCosmosDatabase!.GetContainer(A<string>._))!.Returns(FakeCosmosContainer);
    }

    public IAzureClientFactory<CosmosClient> FakeAzureClientFactory { get; set; }

    public CosmosClient FakeCosmosClient { get; set; }

    public Database FakeCosmosDatabase { get; set; }

    public Container FakeCosmosContainer { get; set; }

    public IOptions<CosmosDbOptions> FakeCosmosDbOptions { get; set; }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupReadItemAsyncSuccess<T>(T itemToReturn, string id) where T : class
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(itemToReturn);
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.OK);

        var containerConfig = A.CallTo(() =>
            FakeCosmosContainer.ReadItemAsync<T>(
                A<string>.That.Matches(x => x == id), A<PartitionKey>._, A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig.Returns(fakeItemResponse);

        return containerConfig;
    }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupReadItemAsyncNotFound<T>(string id) where T : class
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var containerConfig = A.CallTo(() =>
            FakeCosmosContainer.ReadItemAsync<T>(
                A<string>.That.Matches(x => x == id), A<PartitionKey>._, A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig.Throws(
            new CosmosException("item was not found", HttpStatusCode.NotFound, 0, "activityId", 0));

        return containerConfig;
    }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupReadItemAsyncGenericException<T>(string id) where T : class
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var containerConfig = A.CallTo(() =>
            FakeCosmosContainer.ReadItemAsync<T>(
                A<string>.That.Matches(x => x == id), A<PartitionKey>._, A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig.Throws(
            new Exception("something bad happened"));

        return containerConfig;
    }

    public IAnyCallConfigurationWithReturnTypeSpecified<Task<ItemResponse<T>>> SetupUpsertItemAsyncSuccess<T>(
        T itemToReturn) where T : class
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(itemToReturn);
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.OK);

        var containerConfig = A.CallTo(FakeCosmosContainer).Where(call => call.Method.Name == "UpsertItemAsync")
            .WithReturnType<Task<ItemResponse<T>>>();
        containerConfig.Returns(fakeItemResponse);

        return containerConfig;
    }

    public IAnyCallConfigurationWithReturnTypeSpecified<Task<ItemResponse<T>>> SetupUpsertItemAsyncNotFound<T>()
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.NotFound);

        var containerConfig = A.CallTo(FakeCosmosContainer).Where(call => call.Method.Name == "UpsertItemAsync")
            .WithReturnType<Task<ItemResponse<T>>>();
        containerConfig.Returns(fakeItemResponse);

        return containerConfig;
    }

    public IAnyCallConfigurationWithReturnTypeSpecified<Task<ItemResponse<T>>> SetupUpsertItemAsyncGenericException<T>()
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.NotFound);

        var containerConfig = A.CallTo(FakeCosmosContainer).Where(call => call.Method.Name == "UpsertItemAsync")
            .WithReturnType<Task<ItemResponse<T>>>();
        containerConfig.Throws(new Exception("Something bad happened"));

        return containerConfig;
    }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupCreateItemAsyncSuccess<T>(T itemToReturn) where T : class
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(itemToReturn);
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.OK);

        var containerConfig = A.CallTo(() =>
            FakeCosmosContainer.CreateItemAsync(
                A<T>._, A<PartitionKey>._, A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig.Returns(value: fakeItemResponse);

        return containerConfig;
    }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupCreateItemAsyncGenericException<T>()
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.NotFound);

        var containerConfig = A.CallTo(() =>
            FakeCosmosContainer.CreateItemAsync(
                A<T>._, A<PartitionKey>._, A<ItemRequestOptions>._, A<CancellationToken>._));
        containerConfig.Throws(new Exception("Something bad happened"));

        return containerConfig;
    }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupDeleteItemAsyncSuccess<T>(T itemToReturn) where T : class
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(itemToReturn);
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.OK);

        var containerConfig = A.CallTo(() =>
           FakeCosmosContainer.PatchItemAsync<T>(
               A<string>._, A<PartitionKey>._, A<IReadOnlyList<PatchOperation>>._, null, A<CancellationToken>._));
        containerConfig.Returns(fakeItemResponse);

        return containerConfig;
    }

    public IReturnValueArgumentValidationConfiguration<Task<ItemResponse<T>>> SetupDeleteItemAsyncGenericException<T>()
    {
        Fake.ClearRecordedCalls(FakeCosmosContainer);

        var fakeItemResponse = A.Fake<ItemResponse<T>>();
        A.CallTo(() => fakeItemResponse.StatusCode).Returns(HttpStatusCode.NotFound);

        var containerConfig = A.CallTo(() =>
           FakeCosmosContainer.PatchItemAsync<T>(
               A<string>._, A<PartitionKey>._, A<IReadOnlyList<PatchOperation>>._, null, A<CancellationToken>._));
        containerConfig.Throws(new Exception("Something bad happened"));

        return containerConfig;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        FakeCosmosClient.Dispose();
        Fake.ClearRecordedCalls(FakeCosmosContainer);
    }
}