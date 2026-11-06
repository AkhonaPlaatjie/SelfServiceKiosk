using SelfServiceKiosk.Api.DTOs.Profile;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(IUserRepository userRepository, ILogger<ProfileService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ProfileDto> GetAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetActiveUserAsync(userId, cancellationToken);

        return user.ToProfileDto();
    }

    public async Task<ProfileDto> UpdateAsync(
        int userId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await GetActiveUserAsync(userId, cancellationToken);

        // Only check for a duplicate when the email really changes (so keeping your own email is fine).
        var newEmail = request.Email.Trim().ToLowerInvariant();
        if (newEmail != user.Email && await _userRepository.EmailExistsAsync(newEmail, cancellationToken))
        {
            // Never log the email address (NFR-09).
            _logger.LogWarning("Profile update refused for user {UserId}: the email is already registered.", userId);
            throw new EmailAlreadyExistsException();
        }

        user.FirstName = request.FirstName.Trim();
        user.Surname = request.Surname.Trim();
        user.Email = newEmail;
        user.MobileNumber = request.MobileNumber.Trim();

        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} updated their profile.", userId);

        return user.ToProfileDto();
    }

    private async Task<User> GetActiveUserAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            // A token for an account that no longer exists or was deactivated no longer works here.
            throw new InvalidCredentialsException();
        }

        return user;
    }
}
