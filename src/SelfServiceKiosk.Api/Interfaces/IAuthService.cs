using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Business rules for authentication.</summary>
public interface IAuthService
{
    /// <summary>
    /// Checks the credentials and returns an access token.
    /// Throws InvalidCredentialsException for an unknown email, a wrong password or the wrong role,
    /// and AccountDeactivatedException for a deactivated account (BR-09).
    /// </summary>
    /// <param name="request">Email and password.</param>
    /// <param name="requiredRole">The role the account must hold for this login endpoint.</param>
    /// <param name="cancellationToken">Cancels the database call if the client disconnects.</param>
Task<LoginResponseDto> LoginAsync(
    LoginRequestDto request,
    UserRole? requiredRole = null,
    CancellationToken cancellationToken = default);

/// <summary>Issues a fresh token for an active account. Throws InvalidCredentialsException otherwise.</summary>
Task<LoginResponseDto> RefreshAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
/// Creates a new account with the User role. Throws EmailAlreadyExistsException for a duplicate email.
/// </summary>
Task<UserSummaryDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);   
}
