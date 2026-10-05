using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>What the client sends to a login endpoint.</summary>
public class LoginRequestDto
{
    /// <example>user@kiosk.test</example>
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;
}
