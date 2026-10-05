using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>The parts of an account that are safe to send to the client. No password hash.</summary>
public class UserSummaryDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}
