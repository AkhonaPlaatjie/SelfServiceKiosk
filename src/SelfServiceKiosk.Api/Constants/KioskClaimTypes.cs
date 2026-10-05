namespace SelfServiceKiosk.Api.Constants;

/// <summary>
/// The claim names used inside the JWT. Keeping them in one place means the code that
/// writes the token (TokenService) and the code that reads it (JWT bearer setup) cannot drift apart.
/// </summary>
public static class KioskClaimTypes
{
    /// <summary>The user's database Id. "sub" is the standard JWT name for the token's subject.</summary>
    public const string UserId = "sub";

    /// <summary>The user's role: the name of a UserRole enum value.</summary>
    public const string Role = "role";
}
