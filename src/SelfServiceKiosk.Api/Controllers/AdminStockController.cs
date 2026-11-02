using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.DTOs.Products;
using SelfServiceKiosk.Api.DTOs.Stock;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>Stock purchases are audit records: there is deliberately no PUT or DELETE.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = KioskPolicies.Administrator)]
public class AdminStockController : ControllerBase
{
    private readonly IStockService _stockService;

    public AdminStockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(KioskClaimTypes.UserId)!, CultureInfo.InvariantCulture);

    [HttpPost("stock-purchases")]
    public async Task<ActionResult<StockPurchaseDto>> Record(
        [FromBody] RecordPurchaseRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _stockService.RecordPurchaseAsync(request, CurrentUserId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("stock-purchases")]
    public async Task<ActionResult<PagedResult<StockPurchaseDto>>> GetPage(
        [FromQuery] PageQuery query,
        [FromQuery] int? productId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        return Ok(await _stockService.GetPageAsync(query, productId, fromUtc, toUtc, cancellationToken));
    }

    [HttpGet("stock-purchases/{id:int}")]
    public async Task<ActionResult<StockPurchaseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _stockService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost("stock-purchases/{id:int}/corrections")]
    public async Task<ActionResult<StockPurchaseDto>> Correct(
        int id,
        [FromBody] RecordCorrectionRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _stockService.RecordCorrectionAsync(id, request, CurrentUserId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetLowStock(CancellationToken cancellationToken)
    {
        return Ok(await _stockService.GetLowStockAsync(cancellationToken));
    }
}
