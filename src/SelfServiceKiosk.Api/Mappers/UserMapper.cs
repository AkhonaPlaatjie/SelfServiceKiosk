using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Mappers;

/// <summary>
/// Converts User entities into DTOs. Mapping lives here, in one place,
/// so that services stay about business rules and controllers stay about HTTP.
/// </summary>
public static class UserMapper
{
    public static UserSummaryDto ToSummaryDto(this User user)
    {
        return new UserSummaryDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Surname = user.Surname,
            Email = user.Email,
            Role = user.Role,
        };
    }

    public static LoginResponseDto ToLoginResponseDto(this User user, TokenResult token)
    {
        return new LoginResponseDto
        {
            AccessToken = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = user.ToSummaryDto(),
        };
    }
}
