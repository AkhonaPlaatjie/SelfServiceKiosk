using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.DTOs.Profile;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>
/// A user's own profile. The user is always identified by the token, never by the URL or the body,
/// so nobody can read or change anyone else's details. A valid token is required (no [AllowAnonymous]).
/// </summary>
[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(KioskClaimTypes.UserId)!, CultureInfo.InvariantCulture);

    /// <summary>Returns the logged-in user's details.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProfileDto>> Get(CancellationToken cancellationToken)
    {
        return Ok(await _profileService.GetAsync(CurrentUserId, cancellationToken));
    }

    /// <summary>Updates the logged-in user's details.</summary>
    [HttpPut]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProfileDto>> Update(
        [FromBody] UpdateProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        return Ok(await _profileService.UpdateAsync(CurrentUserId, request, cancellationToken));
    }
}
