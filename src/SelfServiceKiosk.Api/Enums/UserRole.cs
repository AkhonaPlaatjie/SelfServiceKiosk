namespace SelfServiceKiosk.Api.Enums;

/// <summary>
/// The roles an account can hold. Stored in the database as text ("User" / "SuperUser")
/// and written into the JWT as the "role" claim.
/// </summary>
public enum UserRole
{
    /// <summary>A registered customer using the kiosk.</summary>
    User = 1,

    /// <summary>The kiosk owner or a staff member who manages the system.</summary>
    SuperUser = 2,
}
