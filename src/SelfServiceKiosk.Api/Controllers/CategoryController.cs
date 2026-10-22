using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.DTOs.Categories;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>
/// Category endpoints. A controller only deals with HTTP: it calls a service and returns a DTO.
/// There is no [AllowAnonymous], so a valid token is required (NFR-02).
/// </summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>Lists every product category, ordered by name.</summary>
    /// <response code="200">The categories.</response>
    /// <response code="401">No valid access token was supplied.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }
}
