using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromotionsEngine.Application.Queries;
using PromotionsEngine.Application.QueryHandlers.Interfaces;

namespace PromotionsEngine.WebApi.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class OffersController : ControllerBase
{
    private readonly IGetOffersForCheckoutQueryHandler _checkoutQueryHandler;
    private readonly IGetOffersForAppQueryHandler _appQueryHandler;
    private readonly ILogger<OffersController> _logger;

    public OffersController(
        IGetOffersForCheckoutQueryHandler checkoutQueryHandler, 
        IGetOffersForAppQueryHandler appQueryHandler,
        ILogger<OffersController> logger)
    {
        _checkoutQueryHandler = checkoutQueryHandler;
        _appQueryHandler = appQueryHandler;
        _logger = logger;
    }

    /// <summary>
    /// This endpoint will return all the valid offers that are available for the provided merchant.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("checkout")]
    public async Task<IActionResult> GetOffersForCheckout([FromQuery]GetOffersForCheckoutQuery query, CancellationToken cancellationToken)
    {
        try
        {
            if (query == null || string.IsNullOrEmpty(query.MerchantId) || query.OrderAmount == 0)
            {
                return BadRequest();
            }

            var results = await _checkoutQueryHandler.GetOffersForCheckoutAsync(query, cancellationToken);

            return Ok(results);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered attempting to get offers for checkout");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// This method will return all the currently active promotions for each merchant.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("app")]
    public async Task<IActionResult> GetOffersForApp([FromQuery] GetOffersForAppQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var results = await _appQueryHandler.GetOffersForAppAsync(query, cancellationToken);

            //We don't need to check for empty here because when doing a GET for a list , it is expected to return an empty list if no results are found.
            return Ok(results);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception encountered when attempting to get offers for app");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}