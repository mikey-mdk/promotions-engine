using AutoFixture;
using FluentAssertions;
using System.ComponentModel;
using PromotionsEngine.Application.Dtos.Merchant;
using PromotionsEngine.Application.Exceptions;
using PromotionsEngine.Application.Mappers;
using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Application.Services.Implementations;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Domain.Repositories.Interfaces;
using PromotionsEngine.Domain.Requests;
using Assert = Xunit.Assert;

namespace PromotionsEngine.Tests.Application.Services;

public class MerchantServiceTests
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IMerchantService _merchantService;

    private readonly Fixture _fixture;

    public MerchantServiceTests()
    {
        _merchantRepository = A.Fake<IMerchantRepository>();
        _fixture = new Fixture();

        _merchantService = new MerchantService(_merchantRepository);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.GetMerchantByIdAsync))]
    [Description($"Test {nameof(MerchantService.GetMerchantByIdAsync)} returns a {nameof(MerchantDto)} when a Merchant is found")]
    public async Task Test_Get_Merchant_By_Id_Async_Success()
    {
        string merchantId = Guid.NewGuid().ToString();
        const string merchantName = "test merchant";

        var expectedMerchant = new Merchant
        {
            Id = merchantId,
            MerchantName = merchantName,
            BusinessType = "Electronics",
            MerchantType = "Brand",
        };

        var expectedMerchantDto = _fixture.Build<MerchantDto>()
                                       .With(x => x.Id, expectedMerchant.Id)
                                       .With(x => x.MerchantName, expectedMerchant.MerchantName)
                                       .With(x => x.BusinessType, expectedMerchant.BusinessType)
                                       .With(x => x.MerchantType, expectedMerchant.MerchantType)
                                       .Create();

        var merchantRepoConfig = A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(A<string>._, default));
        merchantRepoConfig.Returns(expectedMerchant);

        var result = await _merchantService.GetMerchantByIdAsync(merchantId, default);

        merchantRepoConfig.MustHaveHappenedOnceExactly();

        // Assert
        Assert.Equal(expectedMerchantDto.Id, result.Id);
        Assert.Equal(expectedMerchantDto.MerchantName, result.MerchantName);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.GetMerchantByIdAsync))]
    [Description($"Test {nameof(MerchantService.GetMerchantByIdAsync)} returns an empty {nameof(MerchantDto)} when a Merchant is not found")]
    public async Task Test_Get_Merchant_By_Id_Async_Not_Found()
    {
        var merchantRepoConfig = A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(A<string>._, default));
        merchantRepoConfig.Returns((Merchant?)null);

        var result = await _merchantService.GetMerchantByIdAsync(Guid.NewGuid().ToString(), default);

        merchantRepoConfig.MustHaveHappenedOnceExactly();
        Assert.Empty(result.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.GetMerchantByIdAsync))]
    [Description($"Test {nameof(MerchantService.GetMerchantByIdAsync)} returns an empty {nameof(MerchantDto)} when an Exception occurs")]
    public async Task Get_Merchant_By_Id_Async_Exception()
    {
        // Arrange
        var merchantRepoConfig = A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(A<string>._, default));
        merchantRepoConfig.Throws(new Exception("this is a test exception"));

        // Act
        var result = await Assert.ThrowsAsync<Exception>(() => _merchantService.GetMerchantByIdAsync("test", default));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Exception>(result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.CreateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.CreateMerchantAsync)} which create {nameof(MerchantDto)}")]
    public async Task CreateMerchantAsync_ShouldCreateMerchantAndReturnMappedDto()
    {
        // Arrange
        var request = _fixture.Create<CreateMerchantRequest>();

        var createMerchantCall = A.CallTo(() => _merchantRepository.CreateMerchantAsync(A<Merchant>._, default));
        createMerchantCall!.ReturnsLazily((Merchant m, CancellationToken _) => Task.FromResult(m));

        // Act
        var result = await _merchantService.CreateMerchantAsync(request, default);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrEmpty(result.Id));
        createMerchantCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.CreateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.CreateMerchantAsync)} which Throw Exeption")]
    public async Task CreateMerchantAsync_Throw_Exception()
    {
        // Arrange
        var request = _fixture.Create<CreateMerchantRequest>();

        var createMerchantCall = A.CallTo(() => _merchantRepository.CreateMerchantAsync(A<Merchant>._, default));
        createMerchantCall.Throws(new Exception("this is a test exception"));

        // Act
        var result = await Assert.ThrowsAsync<Exception>(() => _merchantService.CreateMerchantAsync(request, default));

        // Assert
        Assert.IsType<Exception>(result);
        createMerchantCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.CreateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.CreateMerchantAsync)} throws exception when Merchant Repository returns null")]
    public async Task When_MerchantRepositoryReturnsNull_CreateMerchantAsync_ThrowsExceptions()
    {
        // Arrange
        var request = _fixture.Create<CreateMerchantRequest>();

        var createMerchantCall = A.CallTo(() => _merchantRepository.CreateMerchantAsync(A<Merchant>._, default));
        createMerchantCall.Returns((Merchant?)null);

        // Act
        var result = await Assert.ThrowsAsync<DomainObjectNullException>(() => _merchantService.CreateMerchantAsync(request, default));

        // Assert
        Assert.IsType<DomainObjectNullException>(result);
        createMerchantCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.UpdateMerchantAsync)} return updated record")]
    public async Task UpdateMerchantAsync_ShouldUpdateMerchantAndReturnMappedDto()
    {
        // Arrange
        var request = _fixture.Create<UpdateMerchantRequest>();

        var existingMerchant = _fixture.Build<Merchant>()
                                        .With(x => x.Id, request.Id)
                                        .Create();

        var updatedMerchant = new Merchant
        {
            Id = request.Id,
            ExternalMerchantId = request.ExternalMerchantId!,
            MerchantName = request.MerchantName!,
            Description = request.Description!,
            MerchantType = request.MerchantType!,
            BusinessType = request.BusinessType!,
            Active = request.Active,
            ModifiedDateTime = DateTime.UtcNow
        };

        var getCall = A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(updatedMerchant.Id, default));
        getCall.Returns(existingMerchant);

        var updateCall = A.CallTo(() => _merchantRepository.UpdateMerchantAsync(A<Merchant>._, default));
        updateCall.Returns(updatedMerchant);

        // Act
        var result = await _merchantService.UpdateMerchantAsync(request, default);

        // Assert
        getCall.MustHaveHappenedOnceExactly();
        updateCall.MustHaveHappenedOnceExactly();

        Assert.Equal(request.ExternalMerchantId, updatedMerchant.ExternalMerchantId);
        Assert.Equal(request.MerchantName, updatedMerchant.MerchantName);
        Assert.Equal(request.Description, updatedMerchant.Description);
        Assert.Equal(request.MerchantType, updatedMerchant.MerchantType);
        Assert.Equal(request.BusinessType, updatedMerchant.BusinessType);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.UpdateMerchantAsync)} when {nameof(IMerchantRepository.GetMerchantByIdAsync)} method throw exception")]

    public async Task UpdateMerchantAsync_When_GetMerchantByIdAsync_ExceptionThrown_ShouldRethrow()
    {
        // Arrange
        var request = _fixture.Create<UpdateMerchantRequest>();


        A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(request.Id, default))
            .Throws(new Exception("exception thrown"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _merchantService.UpdateMerchantAsync(request, default);
        });
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.UpdateMerchantAsync)} when {nameof(IMerchantRepository.GetMerchantByIdAsync)} returns empty merchant")]
    public async Task UpdateMerchantAsync_When_GetMerchantByIdAsync_ReturnsNull_ShouldThrowDomainObjectNullException()
    {
        // Arrange
        var request = _fixture.Create<UpdateMerchantRequest>();

        var getCall = A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(request.Id, default));
        getCall.Returns((Merchant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainObjectNullException>(async () =>
        {
            await _merchantService.UpdateMerchantAsync(request, default);
        });
        getCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.UpdateMerchantAsync))]
    [Description($"Test {nameof(MerchantService.UpdateMerchantAsync)} when {nameof(IMerchantRepository.UpdateMerchantAsync)} returns empty merchant")]
    public async Task UpdateMerchantAsync_When_UpdateMerchantByIdAsync_ReturnsNull_ShouldThrowException()
    {
        // Arrange
        var request = _fixture.Create<UpdateMerchantRequest>();

        var existingMerchant = _fixture.Build<Merchant>()
                                        .With(x => x.Id, request.Id)
                                        .Create();

        var getCall = A.CallTo(() => _merchantRepository.GetMerchantByIdAsync(A<string>._, default));
        getCall.Returns(existingMerchant);

        var updateCall = A.CallTo(() => _merchantRepository.UpdateMerchantAsync(A<Merchant>._, default));
        updateCall.Returns((Merchant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainObjectNullException>(async () =>
        {
            await _merchantService.UpdateMerchantAsync(request, default);
        });
        updateCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.DeleteMerchantAsync))]
    [Description($"Test {nameof(MerchantService.DeleteMerchantAsync)} return success")]
    public async Task DeleteMerchantAsync_ShouldReturnMappedDto()
    {
        // Arrange
        string merchantId = Guid.NewGuid().ToString();

        var deletedMerchant = _fixture.Build<Merchant>()
                                        .With(x => x.Id, merchantId)
                                        .With(x => x.Active, false)
                                        .Create();

        A.CallTo(() => _merchantRepository.DeleteMerchantAsync(merchantId, default))
            .Returns(deletedMerchant);

        // Act
        var result = await _merchantService.DeleteMerchantAsync(merchantId, default);

        // Assert
        A.CallTo(() => _merchantRepository.DeleteMerchantAsync(merchantId, default)).MustHaveHappenedOnceExactly();
        Assert.NotNull(result);
        Assert.Equal(merchantId, result.Id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.DeleteMerchantAsync))]
    [Description($"Test {nameof(MerchantService.DeleteMerchantAsync)} when method throw exception")]
    public async Task DeleteMerchantAsync_WhenExceptionThrown_ShouldRethrow()
    {
        // Arrange
        string merchantId = Guid.NewGuid().ToString();

        A.CallTo(() => _merchantRepository.DeleteMerchantAsync(merchantId, default))
            .Throws(new Exception("exception thrown"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _merchantService.DeleteMerchantAsync(merchantId, default);
        });
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.DeleteMerchantAsync))]
    [Description($"Test {nameof(MerchantService.DeleteMerchantAsync)} throws exception")]
    public async Task DeleteMerchantAsync_Throws_Exception()
    {
        // Arrange
        string merchantId = Guid.NewGuid().ToString();

        var deleteCall = A.CallTo(() => _merchantRepository.DeleteMerchantAsync(merchantId, default));
        deleteCall.Returns((Merchant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainObjectNullException>(async () =>
        {
            await _merchantService.DeleteMerchantAsync(merchantId, default);
        });
        deleteCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantService.PatchMerchantAsync)} happy path")]
    public async Task Test_Patch_Merchant_Async_Success()
    {

        var patchedMerchant = _fixture.Create<Merchant>();

        var repositoryCall = A.CallTo(() =>
            _merchantRepository.PatchMerchantAsync(A<PatchMerchantRequest>._, A<CancellationToken>._));
        repositoryCall.Returns(patchedMerchant);

        var response = await _merchantService.PatchMerchantAsync(_fixture.Create<PatchMerchantRequest>(), default);


        repositoryCall.MustHaveHappenedOnceExactly();

        var expected = patchedMerchant.MapToMerchantDto();
        response.Should().BeEquivalentTo(expected);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Class", nameof(MerchantService))]
    [Trait("Method", nameof(MerchantService.PatchMerchantAsync))]
    [Description($"Test {nameof(MerchantService.PatchMerchantAsync)} returns null")]
    public async Task Test_Patch_Merchant_Async_Returns_Null()
    {

        var repositoryCall = A.CallTo(() =>
            _merchantRepository.PatchMerchantAsync(A<PatchMerchantRequest>._, A<CancellationToken>._));
        repositoryCall.Returns((Merchant?)null);

        await Assert.ThrowsAsync<DomainObjectNullException>(async () =>
        {
            await _merchantService.PatchMerchantAsync(new PatchMerchantRequest(), default);
        });
        repositoryCall.MustHaveHappenedOnceExactly();
    }
}