using PromotionsEngine.Application.Dtos.Offers;
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
public class OffersControllerTests
{

    private readonly HttpClient _client;

    private const string BaseOffersControllerUrl = "http://localhost:5001/promotions-engine/offers";
    private const string SuccessMerchantId = "630650e3-331e-4b8e-9f40-7db07b2aae12";
    private const string SuccessOrderAmount = "100";

    public OffersControllerTests(IntegrationTestBase testBase)
    {
        _client = testBase.HttpClient;
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Offers")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetOffers")]
    [Description("Test that GET offers query is returning successfully")]
    public async Task Test_Get_Offers_Success()
    {
        var url = $"{BaseOffersControllerUrl}/checkout?merchantId={SuccessMerchantId}&orderAmount={SuccessOrderAmount}";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var offerDto = JsonSerializer.Deserialize<GetCheckoutOffersDto>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        offerDto?.CheckoutOffers.ShouldNotBeEmpty();
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Offers")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetOffers")]
    [Description("Test that GET offers query is returning empty dto whend document not found")]
    public async Task Test_Get_Offers_Not_Found()
    {
        var url = $"{BaseOffersControllerUrl}/checkout?merchantId=notfound&orderAmount=100";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var offerDto = JsonSerializer.Deserialize<GetCheckoutOffersDto>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        offerDto?.CheckoutOffers.ShouldBeEmpty();
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Offers")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetOffers")]
    [Description("Test that GET offers query is returning bad request when input params are invalid")]
    public async Task Test_Get_Offers_Bad_Request()
    {
        var url = $"{BaseOffersControllerUrl}/checkout?merchantId={SuccessMerchantId}";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}