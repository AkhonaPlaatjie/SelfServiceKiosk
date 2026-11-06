using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.DTOs.Profile;

/// <summary>A user's own details. It never includes the password or its hash.</summary>
public class ProfileDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
