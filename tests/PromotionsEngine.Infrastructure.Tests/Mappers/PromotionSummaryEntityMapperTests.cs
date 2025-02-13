using System.ComponentModel;
using AutoFixture;
using FluentAssertions;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;
using PromotionsEngine.Infrastructure.Mappers;

namespace PromotionsEngine.Infrastructure.Tests.Mappers;

public class PromotionSummaryEntityMapperTests
{
    private readonly Fixture _fixture;

    public PromotionSummaryEntityMapperTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryEntityMapper))]
    [Trait("Method", "MapToEntity")]
    [Description("Test mapping a domain object to an entity")]
    public void Test_Map_To_Entity()
    {
        var promotionSummary = _fixture.Create<PromotionSummary>();

        var actualEntity = promotionSummary.MapToEntity();

        actualEntity.Should().BeEquivalentTo(promotionSummary);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "Repository")]
    [Trait("Class", nameof(PromotionSummaryEntityMapper))]
    [Trait("Method", "MapToDomain")]
    [Description("Test mapping an entity object to a domain")]
    public void Test_Map_To_Domain()
    {
        var promotionSummaryEntity = _fixture.Create<PromotionSummaryEntity>();

        var actual = promotionSummaryEntity.MapToDomain();

        actual.Should().BeEquivalentTo(promotionSummaryEntity);
    }
}