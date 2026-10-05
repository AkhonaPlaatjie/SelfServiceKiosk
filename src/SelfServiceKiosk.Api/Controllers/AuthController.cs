using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Interfaces;

namespace SelfServiceKiosk.Api.Controllers;

/// <summary>
/// Login endpoints. A controller only deals with HTTP: it receives a DTO, calls a service
/// and returns a DTO. It contains no business rules and never touches the database.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Logs in a customer account (role: User).</summary>
    /// <response code="200">The credentials are correct. Returns an access token.</response>
    /// <response code="400">The request body failed validation.</response>
    /// <response code="401">The email or password is wrong, or the account is not a User.</response>
    /// <response code="403">The account has been deactivated.</response>
    [HttpPost("login/user")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponseDto>> LoginUser(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, UserRole.User, cancellationToken);
        return Ok(response);
    }

    /// <summary>Logs in a staff account (role: SuperUser).</summary>
    /// <response code="200">The credentials are correct. Returns an access token.</response>
    /// <response code="400">The request body failed validation.</response>
    /// <response code="401">The email or password is wrong, or the account is not a SuperUser.</response>
    /// <response code="403">The account has been deactivated.</response>
    [HttpPost("login/superuser")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponseDto>> LoginSuperUser(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, UserRole.SuperUser, cancellationToken);
        return Ok(response);
    }
}
