using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Application.Validation.Implementations;
using PromotionsEngine.Domain.Constants;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.Tests.Application.Validation;

[ExcludeFromCodeCoverage]
public class MerchantValidationEngineTests
{
    public static TheoryData<CreateMerchantRequest, int> CreateTheoryData => new()
    {
        { new CreateMerchantRequest(), 4 },
        { new CreateMerchantRequest
        {
            MerchantName = "Merchant Name"
        }, 3 },
        { new CreateMerchantRequest
        {
            MerchantName = "Merchant Name",
            RegexPatterns = new List<string> { "pattern" }
        }, 2 },
        { new CreateMerchantRequest
        {
            MerchantName = "Merchant Name",
            MerchantType = CMerchantTypes.Brand,
            RegexPatterns = new List<string> { "pattern" }
        }, 1 },
        { new CreateMerchantRequest
        {
            MerchantName = "Merchant Name",
            MerchantType = CMerchantTypes.Retailer,
            BusinessType = CBusinessTypes.Electronics,
            RegexPatterns = new List<string> { "pattern" }
        }, 0 },
    };

    public static TheoryData<UpdateMerchantRequest, int> UpdateTheoryData => new()
    {
        { new UpdateMerchantRequest(), 1 },
        { new UpdateMerchantRequest
        {
            Id = "Id",
            MerchantType = "type",
            BusinessType = "type"
        }, 2 },
        { new UpdateMerchantRequest
        {
            Id = "Id",
            MerchantName = CMerchantTypes.Brand,
            BusinessType = "type"
        }, 1 },
        { new UpdateMerchantRequest
        {
            Id = "Id",
            MerchantType = CMerchantTypes.Brand,
            BusinessType = CBusinessTypes.Electronics
        }, 0 }
    };

    public static TheoryData<PatchMerchantRequest, int> PatchTheoryData => new()
    {
        { new PatchMerchantRequest(), 1 },
        { new PatchMerchantRequest
        {
            Id = "Id",
            MerchantType = "type",
            BusinessType = "type"
        }, 2 },
        { new PatchMerchantRequest
        {
            Id = "Id",
            MerchantName = CMerchantTypes.Brand,
            BusinessType = "type"
        }, 1 },
        { new PatchMerchantRequest
        {
            Id = "Id",
            MerchantType = CMerchantTypes.Brand,
            BusinessType = CBusinessTypes.Electronics
        }, 0 }
    };

    [Theory]
    [MemberData(nameof(CreateTheoryData))]
    [Trait("Class", nameof(MerchantValidationEngine))]
    [Trait("Category", "Unit")]
    [Trait("Method", nameof(MerchantValidationEngine.Validate))]
    [Description("Test validate create merchant request")]
    public void Test_Validate_Create_Merchant_Request(CreateMerchantRequest request, int numberOfValidationMessages)
    {
        // Arrange
        var merchantValidationEngine = new MerchantValidationEngine();

        // Act
        var result = merchantValidationEngine.Validate(request);

        // Assert
        result.Count.Should().Be(numberOfValidationMessages);
    }

    [Theory]
    [MemberData(nameof(UpdateTheoryData))]
    [Trait("Class", nameof(MerchantValidationEngine))]
    [Trait("Category", "Unit")]
    [Trait("Method", nameof(MerchantValidationEngine.Validate))]
    [Description("Test validate update merchant request")]
    public void Test_Validate_Update_Merchant_Request(UpdateMerchantRequest request, int numberOfValidationMessages)
    {
        // Arrange
        var merchantValidationEngine = new MerchantValidationEngine();

        // Act
        var result = merchantValidationEngine.Validate(request);

        // Assert
        result.Count.Should().Be(numberOfValidationMessages);
    }

    [Theory]
    [MemberData(nameof(PatchTheoryData))]
    [Trait("Class", nameof(MerchantValidationEngine))]
    [Trait("Category", "Unit")]
    [Trait("Method", nameof(MerchantValidationEngine.Validate))]
    [Description("Test validate patch merchant request")]
    public void Test_Validate_Patch_Merchant_Request(PatchMerchantRequest request, int numberOfValidationMessages)
    {
        // Arrange
        var merchantValidationEngine = new MerchantValidationEngine();

        // Act
        var result = merchantValidationEngine.Validate(request);

        // Assert
        result.Count.Should().Be(numberOfValidationMessages);
    }
}