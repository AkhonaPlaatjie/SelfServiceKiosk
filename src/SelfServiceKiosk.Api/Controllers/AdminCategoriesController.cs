using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.DTOs.Categories;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>Category management for Administrators. There is no delete endpoint: categories are deactivated.</summary>
[ApiController]
[Route("api/admin/categories")]
[Authorize(Policy = KioskPolicies.Administrator)]
public class AdminCategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public AdminCategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CategoryDto>>> GetPage(
        [FromQuery] PageQuery query,
        [FromQuery] bool includeInactive = true,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _categoryService.GetPageAsync(query, includeInactive, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] SaveCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _categoryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(
        int id,
        [FromBody] SaveCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.UpdateAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:int}/deactivate")]
    public async Task<ActionResult<CategoryDto>> Deactivate(int id, CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.SetActiveAsync(id, false, cancellationToken));
    }

    [HttpPost("{id:int}/reactivate")]
    public async Task<ActionResult<CategoryDto>> Reactivate(int id, CancellationToken cancellationToken)
    {
        return Ok(await _categoryService.SetActiveAsync(id, true, cancellationToken));
    }
}