namespace SelfServiceKiosk.Api.Models;

/// <summary>A signed access token and the moment it stops being valid.</summary>
public record TokenResult(string Token, DateTime ExpiresAtUtc);
