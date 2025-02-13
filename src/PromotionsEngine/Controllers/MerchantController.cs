using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromotionsEngine.Application.Exceptions;
using PromotionsEngine.Application.Requests.Merchant;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Application.Validation.Interfaces;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.WebApi.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class MerchantController : ControllerBase
{
    private readonly IMerchantService _merchantService;
    private readonly IMerchantValidationEngine _merchantValidationEngine;

    public MerchantController(
        IMerchantService merchantService,
        IMerchantValidationEngine merchantValidationEngine)
    {
        _merchantService = merchantService;
        _merchantValidationEngine = merchantValidationEngine;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMerchantById([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var merchantDto = await _merchantService.GetMerchantByIdAsync(id, cancellationToken);

        return string.IsNullOrEmpty(merchantDto.Id) ? NotFound() : Ok(merchantDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMerchant([FromBody] CreateMerchantRequest request, CancellationToken cancellationToken)
    {
        var validationResults = _merchantValidationEngine.Validate(request);

        if (validationResults.Count > 0)
        {
            return BadRequest(validationResults);
        }

        var merchantDto = await _merchantService.CreateMerchantAsync(request, cancellationToken);

        return Ok(merchantDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMerchant([FromBody] UpdateMerchantRequest request, CancellationToken cancellationToken)
    {
        var validationResults = _merchantValidationEngine.Validate(request);

        if (validationResults.Count > 0)
        {
            return BadRequest(validationResults);
        }

        try
        {
            var merchantDto = await _merchantService.UpdateMerchantAsync(request, cancellationToken);

            return Ok(merchantDto);
        }
        catch (DomainObjectNullException)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }
    }

    [HttpPatch]
    public async Task<IActionResult> PatchMerchant([FromBody] PatchMerchantRequest request,
        CancellationToken cancellationToken)
    {
        var validationResults = _merchantValidationEngine.Validate(request);

        if (validationResults.Count > 0)
        {
            return BadRequest(validationResults);
        }

        if (string.IsNullOrEmpty(request.Id))
        {
            return BadRequest();
        }

        try
        {
            var merchantDto = await _merchantService.PatchMerchantAsync(request, cancellationToken);

            return Ok(merchantDto);
        }
        catch (Exception)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMerchant([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var merchantDto = await _merchantService.DeleteMerchantAsync(id, cancellationToken);

        return Ok(merchantDto);
    }
}