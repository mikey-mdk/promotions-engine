using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromotionsEngine.Application.Requests.Promotions;
using PromotionsEngine.Application.Services.Interfaces;
using PromotionsEngine.Domain.Requests;

namespace PromotionsEngine.WebApi.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionsService _promotionsService;
    private readonly ILogger<PromotionsController> _logger;

    public PromotionsController(
        IPromotionsService promotionsService, 
        ILogger<PromotionsController> logger)
    {
        _promotionsService = promotionsService;
        _logger = logger;
    }

    [HttpGet("{promotionId}")]
    public async Task<IActionResult> GetPromotionById([FromRoute] string promotionId, CancellationToken cancellationToken)
    {
        if (promotionId == default)
        {
            return BadRequest();
        }

        var promotion = await _promotionsService.GetPromotionByIdAsync(promotionId, cancellationToken);

        return string.IsNullOrWhiteSpace(promotion.Id) ? NotFound() : Ok(promotion);
    }

    [HttpGet]
    public async Task<IActionResult> GetPromotions([FromQuery] GetPromotionsQueryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
            {
                return BadRequest();
            }

            var results = await _promotionsService.GetPromotionsFromQueryAsync(request, cancellationToken);

            return Ok(results);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered when attempting to query promotions");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionsService.CreatePromotionAsync(request, cancellationToken);

        return Ok(promotion);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePromotion([FromBody] UpdatePromotionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            return BadRequest();
        }

        var promotion = await _promotionsService.UpdatePromotionAsync(request, cancellationToken);

        return Ok(promotion);
    }

    [HttpDelete("{promotionId}")]
    public async Task<IActionResult> DeletePromotion([FromRoute] string promotionId, CancellationToken cancellationToken)
    {
        if (promotionId == default)
        {
            return BadRequest();
        }

        var promotion = await _promotionsService.DeletePromotionAsync(promotionId, cancellationToken);

        return Ok(promotion);
    }
}