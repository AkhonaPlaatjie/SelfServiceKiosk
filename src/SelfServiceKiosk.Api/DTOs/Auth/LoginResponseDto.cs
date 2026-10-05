namespace SelfServiceKiosk.Api.DTOs.Auth;

/// <summary>What a login endpoint returns when the credentials are correct.</summary>
public class LoginResponseDto
{
    /// <summary>Send this on later requests in the header: Authorization: Bearer (token)</summary>
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public UserSummaryDto User { get; set; } = new();
}
