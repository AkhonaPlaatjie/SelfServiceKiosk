using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Business rules for authentication and registration.</summary>
public interface IAuthService
{
    /// <summary>
    /// Checks the credentials and returns an access token.
    /// When requiredRole is given, the account must hold that role; when it is null, any role may log in.
    /// </summary>
    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        UserRole? requiredRole = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new account with the User role.
    /// Throws EmailAlreadyExistsException for a duplicate email.
    /// </summary>
    Task<UserSummaryDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default);

    /// <summary>Issues a fresh token for an active account.</summary>
    Task<LoginResponseDto> RefreshAsync(
        int userId,
        CancellationToken cancellationToken = default);
}
