using SelfServiceKiosk.Api.DTOs.Profile;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Mappers;

public static class ProfileMapper
{
    public static ProfileDto ToProfileDto(this User user)
    {
        return new ProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Surname = user.Surname,
            Email = user.Email,
            MobileNumber = user.MobileNumber,
            Role = user.Role,
            CreatedAtUtc = user.CreatedAtUtc,
        };
    }
}
