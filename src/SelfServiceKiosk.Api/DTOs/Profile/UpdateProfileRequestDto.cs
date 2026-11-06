using SelfServiceKiosk.Api.DTOs.Auth;

namespace SelfServiceKiosk.Api.DTOs.Profile;

/// <summary>
/// What a user sends to change their own details. It has the same rules as registration (inherited).
/// There is deliberately no Id, Role or IsActive: the user is identified by the token, and nothing here can change a role.
/// </summary>
public class UpdateProfileRequestDto : PersonalDetailsRequestDto
{
}
