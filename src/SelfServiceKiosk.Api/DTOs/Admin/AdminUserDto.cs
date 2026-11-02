using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.DTOs.Admin;

/// <summary>Only the personal information an administrator needs: no password hash and no mobile number.</summary>
public class AdminUserDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
