using AutoFixture;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;

namespace PromotionsEngine.Tests.Infrastructure.Mappers;

public class MerchantEntityMapperTests
{
    private readonly Fixture _fixture;

    public MerchantEntityMapperTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void MapToEntity_ReturnsExpectedEntity()
    {
        // Arrange
        var merchant = _fixture.Create<Merchant>();

        // Act
        var actualEntity = merchant.MapToEntity();

        // Assert
        Assert.Equal(merchant.Id, actualEntity.Id);
        Assert.Equal(merchant.MerchantName, actualEntity.MerchantName);
        Assert.Equal(merchant.MerchantId, actualEntity.MerchantId);
        Assert.Equal(merchant.MerchantType, actualEntity.MerchantType);
        Assert.Equal(merchant.Active, actualEntity.Active);
        Assert.Equal(merchant.ExternalMerchantId, actualEntity.ExternalMerchantId);
        Assert.Equal(merchant.Deleted, actualEntity.Deleted);
        Assert.Equal(merchant.BusinessType, actualEntity.BusinessType);
        Assert.Equal(merchant.CreatedDateTime, actualEntity.CreatedDateTime);
        Assert.Equal(merchant.Description, actualEntity.Description);
        Assert.Equal(merchant.ModifiedDateTime, actualEntity.ModifiedDateTime);
        Assert.Equal(merchant.SchemaVersion, actualEntity.SchemaVersion);
        Assert.Equivalent(merchant.MerchantAddress, actualEntity.MerchantAddress);
    }

    [Fact]
    public void MapToDomain_ReturnsExpectedDomain()
    {
        // Arrange
        var merchantEntity = _fixture.Create<MerchantEntity>();

        // Act
        var actualDomain = merchantEntity.MapToDomain();

        // Assert
        Assert.Equal(merchantEntity.Id, actualDomain.Id);
        Assert.Equal(merchantEntity.MerchantName, actualDomain.MerchantName);
        Assert.Equal(merchantEntity.MerchantId, actualDomain.MerchantId);
        Assert.Equal(merchantEntity.MerchantType, actualDomain.MerchantType);
        Assert.Equal(merchantEntity.Active, actualDomain.Active);
        Assert.Equal(merchantEntity.ExternalMerchantId, actualDomain.ExternalMerchantId);
        Assert.Equal(merchantEntity.Deleted, actualDomain.Deleted);
        Assert.Equal(merchantEntity.BusinessType, actualDomain.BusinessType);
        Assert.Equal(merchantEntity.CreatedDateTime, actualDomain.CreatedDateTime);
        Assert.Equal(merchantEntity.Description, actualDomain.Description);
        Assert.Equal(merchantEntity.ModifiedDateTime, actualDomain.ModifiedDateTime);
        Assert.Equal(merchantEntity.SchemaVersion, actualDomain.SchemaVersion);
        Assert.Equivalent(merchantEntity.MerchantAddress, actualDomain.MerchantAddress);
    }
}

