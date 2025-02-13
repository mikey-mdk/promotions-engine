using PromotionsEngine.Application.Dtos.Merchant;
using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Tests.Integration.TestBase;
using Shouldly;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Xunit;

namespace PromotionsEngine.Tests.Integration.Controllers;

[Collection("Integration Test Collection")]
[ExcludeFromCodeCoverage]
public class MerchantControllerTests
{
    private readonly HttpClient _client;

    private const string _successMerchantId = "33";
    private const string _baseMerchantControllerUrl = "https://localhost:5001/api/v1/Merchant/";
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public MerchantControllerTests(IntegrationTestBase testBase)
    {
        _client = testBase.HttpClient;
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Merchant")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetMerchantById")]
    [Description("Test that GET merchant by merchant id is returning successfully when a document is found")]
    public async Task Test_Get_Merchant_By_Id_Success()
    {
        var url = $"{_baseMerchantControllerUrl}{_successMerchantId}";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var merchantDto = JsonSerializer.Deserialize<MerchantDto>(content, _serializerOptions);

        merchantDto.ShouldNotBeNull();
        merchantDto.Id.ShouldBe(_successMerchantId);
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Merchant")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "GetMerchantById")]
    [Description("Test that GET merchant by merchant id endpoint is returning not found when no document is found")]
    public async Task Test_Get_Merchant_By_Id_Not_Found()
    {
        var url = $"{_baseMerchantControllerUrl}999";

        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Merchant")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "CreateMerchant")]
    [Description("Test that Create Merchant is returning successfully")]
    public async Task Test_Create_Merchant_Success()
    {
        var response = await CreateTestMerchant();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var merchantDto = JsonSerializer.Deserialize<MerchantDto>(content, _serializerOptions);

        merchantDto.ShouldNotBeNull();
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Merchant")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "UpdateMerchant")]
    [Description("Test that Update Merchant is returning successfully")]
    public async Task Test_Update_Merchant_Success()
    {
        var url = $"{_baseMerchantControllerUrl}";
        var request = new UpdateMerchantRequest
        {
            Id = "33",
            MerchantType = "Retailer",
            BusinessType = "Fashion",
            MerchantName = "Test",
        };

        var jsonContent = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _client.PutAsync(url, stringContent);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        var merchantDto = JsonSerializer.Deserialize<MerchantDto>(content, _serializerOptions);

        merchantDto.ShouldNotBeNull();
        merchantDto.Id.ShouldBe(_successMerchantId);
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Merchant")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "UpdateMerchant")]
    [Description("Test that Update Merchant is returning not found when no document is found")]
    public async Task Test_Update_Merchant_Not_Found()
    {
        var url = $"{_baseMerchantControllerUrl}";
        var request = new UpdateMerchantRequest
        {
            Id = "999",
            MerchantType = "Retailer",
            BusinessType = "Fashion",
            MerchantName = "Test",
        };

        var jsonContent = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        var response = await _client.PutAsync(url, stringContent);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "skipping because the integration tests are currently running on deployment and failing")]
    [Trait("Category", "Merchant")]
    [Trait("Category", "Integration")]
    [Trait("Endpoint", "DeleteMerchant")]
    [Description("Test that Delete Merchant is returning successfully")]
    public async Task Test_Delete_Merchant_Success()
    {
        // Create a test merchant to be deleted
        var response = await CreateTestMerchant();
        var content = await response.Content.ReadAsStringAsync();
        var merchantDto = JsonSerializer.Deserialize<MerchantDto>(content, _serializerOptions);

        // Create delete merchant request
        var url = $"{_baseMerchantControllerUrl}{merchantDto?.Id}";

        response = await _client.DeleteAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        content = await response.Content.ReadAsStringAsync();

        merchantDto = JsonSerializer.Deserialize<MerchantDto>(content, _serializerOptions);

        merchantDto.ShouldNotBeNull();
        merchantDto.Id.ShouldBe(merchantDto.Id);
    }

    private async Task<HttpResponseMessage> CreateTestMerchant()
    {
        var url = $"{_baseMerchantControllerUrl}";
        var request = new CreateMerchantRequest
        {
            Active = true,
            ExternalMerchantId = Guid.NewGuid().ToString(),
            MerchantType = "Retailer",
            MerchantName = "Test Merchant",
            BusinessType = "Fashion",
            MerchantAddress = new MerchantAddressDto()
            {
                AddressLine1 = "123 Somewhere State",
                AddressLine2 = "",
                City = "Whoville",
                State = "NY",
                Country = "US",
                ZipCode = "10001"
            }
        };

        var jsonContent = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);

        return await _client.PostAsync(url, stringContent);
    }
}