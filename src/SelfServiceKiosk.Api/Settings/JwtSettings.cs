using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.Settings;

/// <summary>
/// Bound from the "Jwt" configuration section. The signing key is a secret:
/// it comes from .NET User Secrets or an environment variable, never from Git (NFR-06).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>HMAC-SHA256 needs a key of at least 256 bits, which is 32 characters.</summary>
    [Required(ErrorMessage = "Jwt:Key is not configured. Set it with .NET User Secrets - see README.md.")]
    [MinLength(32, ErrorMessage = "Jwt:Key must be at least 32 characters. Set it with .NET User Secrets - see README.md.")]
    public string Key { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiryMinutes { get; set; } = 30;
}
