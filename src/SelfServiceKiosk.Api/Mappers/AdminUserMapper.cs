using SelfServiceKiosk.Api.DTOs.Admin;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Mappers;

public static class AdminUserMapper
{
    public static AdminUserDto ToAdminDto(this User user)
    {
        return new AdminUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Surname = user.Surname,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc,
        };
    }
}
