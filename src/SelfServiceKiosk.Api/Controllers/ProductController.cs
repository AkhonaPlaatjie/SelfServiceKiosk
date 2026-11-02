using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.DTOs.Products;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>Any logged-in user can read products (customers see only visible ones); only Administrators change them.</summary>
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    private bool IsAdministrator => User.IsInRole(nameof(UserRole.SuperUser));

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetPage(
        [FromQuery] PageQuery query,
        [FromQuery] int? categoryId,
        [FromQuery] bool? isAvailable,
        CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetPageAsync(query, categoryId, isAvailable, IsAdministrator, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _productService.GetByIdAsync(id, IsAdministrator, cancellationToken));
    }

    [HttpPost]
    [Authorize(Policy = KioskPolicies.Administrator)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] SaveProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = KioskPolicies.Administrator)]
    public async Task<ActionResult<ProductDto>> Update(
        int id,
        [FromBody] SaveProductRequestDto request,
        CancellationToken cancellationToken)
    {
        return Ok(await _productService.UpdateAsync(id, request, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = KioskPolicies.Administrator)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
