using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Tests;

/// <summary>Builds fake accounts for tests. Never use a real person's details (NFR-08).</summary>
public static class TestUsers
{
    public static User Create(UserRole role = UserRole.User, bool isActive = true, int id = 1)
    {
        return new User
        {
            Id = id,
            FirstName = "Test",
            Surname = "Account",
            Email = "account@kiosk.test",
            MobileNumber = "0000000000",
            PasswordHash = "not-a-real-hash",
            Role = role,
            IsActive = isActive,
            CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
    }
}
