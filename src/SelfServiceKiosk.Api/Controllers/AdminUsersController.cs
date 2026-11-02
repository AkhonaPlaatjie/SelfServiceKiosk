using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.DTOs.Admin;
using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>User management for Administrators. There is deliberately no delete endpoint.</summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = KioskPolicies.Administrator)]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(KioskClaimTypes.UserId)!, CultureInfo.InvariantCulture);

    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminUserDto>>> GetPage(
        [FromQuery] PageQuery query,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        return Ok(await _adminUserService.GetPageAsync(query, search, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminUserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _adminUserService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<ActionResult<AdminUserDto>> Deactivate(int id, CancellationToken cancellationToken)
    {
        return Ok(await _adminUserService.SetActiveAsync(id, false, CurrentUserId, cancellationToken));
    }

    [HttpPatch("{id:int}/reactivate")]
    public async Task<ActionResult<AdminUserDto>> Reactivate(int id, CancellationToken cancellationToken)
    {
        return Ok(await _adminUserService.SetActiveAsync(id, true, CurrentUserId, cancellationToken));
    }

    [HttpPost("admins")]
    public async Task<ActionResult<AdminUserDto>> CreateAdministrator(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var created = await _adminUserService.CreateAdministratorAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
