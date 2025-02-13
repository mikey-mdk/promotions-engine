using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Requests;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using PromotionsEngine.Tests.Infrastructure.EqualityComparers;
using PromotionsEngine.Tests.Infrastructure.TestBase;

namespace PromotionsEngine.Tests.Infrastructure.Repositories;

public class MerchantRepositoryTests : IClassFixture<RepositoryTestBase>
{
    private readonly ILogger<MerchantRepository> _fakeLogger;
    private readonly RepositoryTestBase _testBase;
    private readonly Fixture _fixture;

    private readonly MerchantRepository _repository;

    public MerchantRepositoryTests(RepositoryTestBase testBase)
    {
        _testBase = testBase;
        _fakeLogger = A.Fake<ILogger<MerchantRepository>>();

        _fixture = new Fixture();

        _repository = new MerchantRepository(testBase.FakeAzureClientFactory, testBase.FakeCosmosDbOptions, _fakeLogger);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.GetMerchantByIdAsync))]
    [Description($"Test {nameof(MerchantRepository.GetMerchantByIdAsync)} returns a {nameof(Merchant)} when a Merchant is found")]
    public async Task Test_GetMerchantByIdAsync_Success()
    {
        var merchantEntity = new MerchantEntity
        {
            Id = "merchantId",
            MerchantName = "test merchant"
        };

        var readItemConfiguration = _testBase.SetupReadItemAsyncSuccess(merchantEntity, merchantEntity.Id);

        var result = await _repository.GetMerchantByIdAsync(merchantEntity.Id, CancellationToken.None);

        readItemConfiguration.MustHaveHappenedOnceExactly();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(merchantEntity.Id, result.Id);
        Assert.Equal(merchantEntity.MerchantName, result.MerchantName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.GetMerchantByIdAsync))]
    [Description($"Test {nameof(MerchantRepository.GetMerchantByIdAsync)} returns null when database returns null")]
    public async Task Test_When_Returns_Null_GetMerchantByIdAsync_Returns_Null()
    {
        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        const string merchantId = "merchantId";

        var readItemConfiguration = A.CallTo(() =>
           _testBase.FakeCosmosContainer.ReadItemAsync<MerchantEntity>(A<string>._, A<PartitionKey>._,
               A<ItemRequestOptions>._, A<CancellationToken>._));
        readItemConfiguration!.Returns((ItemResponse<MerchantEntity>)null!);

        var result = await _repository.GetMerchantByIdAsync(merchantId, CancellationToken.None);

        readItemConfiguration.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.GetMerchantByIdAsync))]
    [Description($"Test {nameof(MerchantRepository.GetMerchantByIdAsync)} returns null when an exception is thrown")]
    public async Task Test_When_Exception_Is_Thrown_GetMerchantByIdAsync_Returns_Null()
    {
        const string merchantId = "merchantId";

        var exceptionConfig =
            _testBase.SetupReadItemAsyncGenericException<MerchantEntity>(merchantId);

        var result = await _repository.GetMerchantByIdAsync(merchantId, CancellationToken.None);

        exceptionConfig.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.CreateMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.CreateMerchantAsync)} returns a {nameof(Merchant)} when a Merchant is created")]
    public async Task Test_CreateMerchantAsync_Success()
    {
        var newMerchant = new Merchant
        {
            Id = "merchantId",
            MerchantName = "test merchant"
        };

        var merchantEntity = newMerchant.MapToEntity();

        var createItemConfiguration =
            _testBase.SetupCreateItemAsyncSuccess(merchantEntity);

        var result = await _repository.CreateMerchantAsync(newMerchant, CancellationToken.None);

        createItemConfiguration.MustHaveHappenedOnceExactly();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(merchantEntity.Id, result.Id);
        Assert.Equal(merchantEntity.MerchantName, result.MerchantName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.CreateMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.CreateMerchantAsync)} returns null when database returns null")]
    public async Task Test_Database_Returns_Null_CreateMerchantAsync_Returns_Null()
    {
        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        var newMerchant = new Merchant
        {
            Id = "merchantId",
            MerchantName = "test merchant"
        };

        var createConfig = A.CallTo(_testBase.FakeCosmosContainer).Where(call => call.Method.Name == "CreateItemAsync")
            .WithReturnType<Task<ItemResponse<MerchantEntity>>>();
        createConfig.Returns((ItemResponse<MerchantEntity>)null!);

        var result = await _repository.CreateMerchantAsync(newMerchant, CancellationToken.None);

        createConfig.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.CreateMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.CreateMerchantAsync)} returns null when an exception is thrown")]
    public async Task Test_When_Exception_Is_Thrown_CreateMerchantAsync_Returns_Null()
    {
        var newMerchant = new Merchant
        {
            Id = "merchantId",
            MerchantName = "test merchant"
        };

        var exceptionConfig = _testBase.SetupCreateItemAsyncGenericException<MerchantEntity>();

        var result = await _repository.CreateMerchantAsync(newMerchant, CancellationToken.None);

        exceptionConfig.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.UpdateMerchantAsync)} returns a {nameof(Merchant)} when a Merchant is found and successfully updated")]
    public async Task Test_UpdateMerchantAsync_Happy_Path()
    {
        const string merchantId = "merchantId";

        var merchantEntity = new MerchantEntity
        {
            Id = merchantId,
            MerchantName = "test merchant"
        };

        var upsertConfig = _testBase.SetupUpsertItemAsyncSuccess(merchantEntity);

        var result = await _repository.UpdateMerchantAsync(new Merchant { Id = merchantId }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(merchantId, result.Id);
        upsertConfig.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.UpdateMerchantAsync)} returns a null when database returns null")]
    public async Task Test_When_Database_Returns_Null_UpdateMerchantAsync_Returns_Null()
    {
        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        var containerConfig = A.CallTo(_testBase.FakeCosmosContainer).Where(call => call.Method.Name == "UpsertItemAsync")
            .WithReturnType<Task<ItemResponse<MerchantEntity>>>();
        containerConfig.Returns((ItemResponse<MerchantEntity>)null!);

        var result = await _repository.UpdateMerchantAsync(new Merchant(), CancellationToken.None);

        // Assert
        containerConfig.MustHaveHappenedOnceExactly();
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.UpdateMerchantAsync)} returns null when an exception is thrown")]
    public async Task Test_When_Exception_Is_Thrown_UpdateMerchantAsync_Returns_Null()
    {
        var exceptionConfig = _testBase.SetupUpsertItemAsyncGenericException<MerchantEntity>();

        var result = await _repository.UpdateMerchantAsync(new Merchant(), CancellationToken.None);

        exceptionConfig.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.DeleteMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.DeleteMerchantAsync)} returns a {nameof(Merchant)} when a Merchant is deleted")]
    public async Task Test_DeleteMerchantAsync_Success()
    {
        const string merchantId = "merchantId";

        var merchantEntity = new MerchantEntity
        {
            Id = merchantId,
            MerchantName = "test merchant"
        };

        var deleteItemConfiguration = _testBase.SetupDeleteItemAsyncSuccess(merchantEntity);

        var result = await _repository.DeleteMerchantAsync(merchantId, CancellationToken.None);

        deleteItemConfiguration.MustHaveHappenedOnceExactly();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(merchantId, result.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.DeleteMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.DeleteMerchantAsync)} returns a null when database returns null")]
    public async Task Test_When_Database_Returns_Null_DeleteMerchantAsync_Returns_Null()
    {
        Fake.ClearRecordedCalls(_testBase.FakeCosmosContainer);

        var deleteConfig = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
               A<string>._, A<PartitionKey>._, A<IReadOnlyList<PatchOperation>>._, null, A<CancellationToken>._));
        deleteConfig.Returns((ItemResponse<MerchantEntity>)null!);

        var result = await _repository.DeleteMerchantAsync("merchantId", CancellationToken.None);

        // Assert
        deleteConfig.MustHaveHappenedOnceExactly();
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.DeleteMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.DeleteMerchantAsync)} returns null when an exception is thrown")]
    public async Task Test_When_Exception_Is_Thrown_DeleteMerchantAsync_Returns_Null()
    {
        var exceptionConfig = _testBase.SetupDeleteItemAsyncGenericException<MerchantEntity>();

        var result = await _repository.DeleteMerchantAsync("merchantId", CancellationToken.None);

        exceptionConfig.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.PatchMerchantAsync)} happy path")]
    public async Task Test_Patch_Merchant_Async_Happy_Path()
    {
        var request = _fixture.Build<PatchMerchantRequest>()
            .Without(x => x.MerchantAddress).Create();

        var patchOperations = new List<PatchOperation>
        {
            PatchOperation.Replace($"/{nameof(MerchantEntity.ExternalMerchantId)}",
                request.ExternalMerchantId),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantName)}",
                request.MerchantName),
            PatchOperation.Replace($"/{nameof(MerchantEntity.Description)}",
                request.Description),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantType)}",
                request.MerchantType),
            PatchOperation.Replace($"/{nameof(MerchantEntity.BusinessType)}",
                request.BusinessType),
            PatchOperation.Replace($"/{nameof(MerchantEntity.Active)}",
                request.Active),
            PatchOperation.Replace($"/{nameof(MerchantEntity.RegexPatterns)}",
                request.RegexPatterns)
        };

        var merchantEntity = _fixture.Create<MerchantEntity>();

        var fakeItemResponse = A.Fake<ItemResponse<MerchantEntity>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(merchantEntity);

        var patchCall = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
            A<string>.That.Matches(x => x == request.Id),
            A<PartitionKey>.That.Matches(x => x == new PartitionKey(request.Id)),
            A<List<PatchOperation>>.That.IsEqualTo(patchOperations, new PatchOperationsEqualityComparer()), A<PatchItemRequestOptions>._,
            A<CancellationToken>._));
        patchCall.Returns(fakeItemResponse);


        var response = await _repository.PatchMerchantAsync(request, CancellationToken.None);

        patchCall.MustHaveHappenedOnceExactly();

        var expected = merchantEntity.MapToDomain();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.PatchMerchantAsync)} MerchantAddress happy path")]
    public async Task Test_Patch_Merchant_Address_Async_Happy_Path()
    {
        var addressRequest = _fixture.Create<PatchMerchantAddressRequest>();

        var patchOperations = new List<PatchOperation>
        {
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.AddressLine1)}",
                addressRequest.AddressLine1),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.AddressLine2)}",
                addressRequest.AddressLine2),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.City)}",
                addressRequest.City),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.State)}",
                addressRequest.State),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.ZipCode)}",
                addressRequest.ZipCode),
            PatchOperation.Replace($"/{nameof(MerchantEntity.MerchantAddress)}/{nameof(MerchantAddressEntity.Country)}",
                addressRequest.Country)
        };

        var patchRequest = new PatchMerchantRequest
        {
            Id = Guid.NewGuid().ToString(),
            MerchantAddress = addressRequest
        };

        var merchantAddressEntity = _fixture.Create<MerchantAddressEntity>();

        var merchantEntity = new MerchantEntity
        {
            Id = patchRequest.Id,
            MerchantAddress = merchantAddressEntity
        };

        var fakeItemResponse = A.Fake<ItemResponse<MerchantEntity>>();
        A.CallTo(() => fakeItemResponse.Resource).Returns(merchantEntity);

        var addressPatchCall = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
            A<string>.That.Matches(x => x == patchRequest.Id),
            A<PartitionKey>.That.Matches(x => x == new PartitionKey(patchRequest.Id)),
            A<List<PatchOperation>>.That.IsEqualTo(patchOperations, new PatchOperationsEqualityComparer()), A<PatchItemRequestOptions>._,
            A<CancellationToken>._));
        addressPatchCall.Returns(fakeItemResponse);

        var patchCall = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
            A<string>.That.Matches(x => x == patchRequest.Id),
            A<PartitionKey>.That.Matches(x => x == new PartitionKey(patchRequest.Id)),
            A<List<PatchOperation>>._, A<PatchItemRequestOptions>._,
            A<CancellationToken>._));
        patchCall.Returns(fakeItemResponse);

        var response = await _repository.PatchMerchantAsync(patchRequest, CancellationToken.None);

        addressPatchCall.MustHaveHappenedOnceExactly();

        var expected = merchantEntity.MapToDomain();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.PatchMerchantAsync)} MerchantAddress throws exception")]
    public async Task Test_Patch_Merchant_Address_Throws_Exception()
    {
        var addressPatchCall = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
            A<string>._,
            A<PartitionKey>._,
            A<List<PatchOperation>>._, A<PatchItemRequestOptions>._,
            A<CancellationToken>._));
        addressPatchCall.Throws(new Exception());

        var response = await _repository.PatchMerchantAsync(new PatchMerchantRequest
        {
            MerchantAddress = _fixture.Create<PatchMerchantAddressRequest>()
        }, CancellationToken.None);

        response.ShouldBeNull();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.PatchMerchantAsync)} throws exception")]
    public async Task Test_Patch_Merchant_Throws_Exception()
    {
        var patch = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
            A<string>._,
            A<PartitionKey>._,
            A<List<PatchOperation>>._, A<PatchItemRequestOptions>._,
            A<CancellationToken>._));
        patch.Throws(new Exception());

        var response = await _repository.PatchMerchantAsync(new PatchMerchantRequest
        {
            MerchantAddress = null,
            Active = true
        }, CancellationToken.None);

        response.ShouldBeNull();

        A.CallTo(_fakeLogger)
            .Where(call => call.Method.Name == "Log" && call.GetArgument<LogLevel>(0) == LogLevel.Error)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(MerchantRepository))]
    [Trait("Method", nameof(MerchantRepository.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantRepository.PatchMerchantAsync)} returns null if no patch options exist")]
    public async Task Test_Patch_Merchant_Returns_Null_With_Zero_Options_To_Patch()
    {
        var patch = A.CallTo(() => _testBase.FakeCosmosContainer.PatchItemAsync<MerchantEntity>(
            A<string>._,
            A<PartitionKey>._,
            A<List<PatchOperation>>._, A<PatchItemRequestOptions>._,
            A<CancellationToken>._));
        patch.Throws(new Exception());

        var response = await _repository.PatchMerchantAsync(new PatchMerchantRequest
        {
            MerchantAddress = null
        }, CancellationToken.None);

        response.ShouldBeNull();
    }
}