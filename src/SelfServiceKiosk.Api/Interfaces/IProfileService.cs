using SelfServiceKiosk.Api.DTOs.Profile;

namespace SelfServiceKiosk.Api.Interfaces;

public interface IProfileService
{
    /// <summary>Returns the details of the account with this id (taken from the token).</summary>
    Task<ProfileDto> GetAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the details of the account with this id (taken from the token).
    /// Throws EmailAlreadyExistsException if the new email belongs to another account.
    /// </summary>
    Task<ProfileDto> UpdateAsync(int userId, UpdateProfileRequestDto request, CancellationToken cancellationToken = default);
}
