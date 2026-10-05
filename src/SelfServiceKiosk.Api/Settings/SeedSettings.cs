namespace SelfServiceKiosk.Api.Settings;

/// <summary>
/// Bound from the "Seed" configuration section. The test accounts' passwords are supplied
/// through .NET User Secrets so that no password is ever committed to Git (NFR-06).
/// </summary>
public class SeedSettings
{
    public const string SectionName = "Seed";

    public string? UserPassword { get; set; }

    public string? SuperUserPassword { get; set; }
}
