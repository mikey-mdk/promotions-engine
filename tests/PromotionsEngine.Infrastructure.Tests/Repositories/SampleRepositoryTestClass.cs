using FakeItEasy;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Repositories.Implementations;
using PromotionsEngine.Tests.Infrastructure.TestBase;

namespace PromotionsEngine.Tests.Infrastructure.Repositories;

public class SampleRepositoryTestClass : IClassFixture<RepositoryTestBase>
{
    private readonly RepositoryTestBase _repositoryTestBase;
    private readonly ILogger<MerchantRepository> _fakeLogger;

    private readonly MerchantRepository _merchantRepository;

    public SampleRepositoryTestClass(RepositoryTestBase repositoryTestBase)
    {
        _repositoryTestBase = repositoryTestBase;

        _fakeLogger = A.Fake<ILogger<MerchantRepository>>();

        _merchantRepository = new MerchantRepository(repositoryTestBase.FakeAzureClientFactory,
            repositoryTestBase.FakeCosmosDbOptions, _fakeLogger);
    }

    [Fact]
    public async Task Test_Successful_Read_On_Container()
    {
        var orderId = Guid.NewGuid().ToString();

        var merchantEntity = new MerchantEntity
        {
            Id = orderId,
            MerchantId = "testmerchantid",
            MerchantName = "testmerchantname",
        };

        var containerConfig = _repositoryTestBase.SetupReadItemAsyncSuccess(merchantEntity, orderId);

        await _merchantRepository.GetMerchantByIdAsync(orderId, default);

        containerConfig.MustHaveHappenedOnceExactly();
    }
}