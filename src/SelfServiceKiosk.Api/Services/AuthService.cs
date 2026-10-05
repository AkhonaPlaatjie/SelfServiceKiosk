using Microsoft.AspNetCore.Identity;
using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Services;

/// <summary>
/// The business rules for logging in. Everything it needs arrives through interfaces,
/// so it can be unit tested without a database or a web server.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        UserRole requiredRole,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            // Never log the email address or the password (NFR-09).
            _logger.LogWarning("Login failed: no account matches the supplied email.");
            throw new InvalidCredentialsException();
        }

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login failed for user {UserId}: wrong password.", user.Id);
            throw new InvalidCredentialsException();
        }

        // A User cannot sign in through the SuperUser endpoint, and the other way round.
        // The response is the same as for a wrong password, so the endpoint reveals nothing.
        if (user.Role != requiredRole)
        {
            _logger.LogWarning(
                "Login failed for user {UserId}: the account is not a {RequiredRole}.",
                user.Id,
                requiredRole);
            throw new InvalidCredentialsException();
        }

        // BR-09: a deactivated user cannot log in. This is only revealed once the password is proven.
        if (!user.IsActive)
        {
            _logger.LogWarning("Login refused for user {UserId}: the account is deactivated.", user.Id);
            throw new AccountDeactivatedException();
        }

        var token = _tokenService.CreateAccessToken(user);

        _logger.LogInformation("User {UserId} logged in as {Role}.", user.Id, user.Role);

        return user.ToLoginResponseDto(token);
    }
}
