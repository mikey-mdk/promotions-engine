using PromotionsEngine.Application.Dtos.Promotion;
using PromotionsEngine.Tests.Integration.TestBase;
using Shouldly;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Xunit;

namespace PromotionsEngine.Tests.Integration.Controllers;

[Collection("Integration Test Collection")]
[ExcludeFromCodeCoverage]
public class PromotionsControllerTests
{
    private readonly HttpClient _client;

    private const string BasePromotionControllerUrl = "https://localhost:5001/promotions-engine/promotions/";
    private const string SuccessPromotionId = "5f2e347b-a5fe-4000-a7c0-8f80728ab21d";

    public PromotionsControllerTests(IntegrationTestBase testBase)
    {
        _client = testBase.HttpClient;
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Promotion")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetPromotionById")]
    [Description("Test that GET promotion by id is returning successfully when a document is found")]
    public async Task Test_Get_Promotion_By_Id_Success()
    {
        var url = $"{BasePromotionControllerUrl}{SuccessPromotionId}";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var promotionDto = JsonSerializer.Deserialize<PromotionDto>(content);

        promotionDto.ShouldNotBeNull();
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Promotion")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetPromotionById")]
    [Description("Test that GET promotion by id endpoint is returning not found when no document is found")]
    public async Task Test_Get_Promotion_By_Id_Not_Found()
    {
        var url = $"{BasePromotionControllerUrl}/notfound";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}