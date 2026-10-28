using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.Models;

/// <summary>
/// Database entity for an account. Entities never leave the API: controllers return DTOs,
/// so fields such as <see cref="PasswordHash"/> can never leak into a response.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;

    /// <summary>Always stored trimmed and lower-case so lookups are case-insensitive.</summary>
    public string Email { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    /// <summary>Produced by ASP.NET Core Identity's password hasher. Never the plain password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>A deactivated account cannot log in, but its history is kept (BR-09).</summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
}
