using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Models;
using SelfServiceKiosk.Api.Settings;

namespace SelfServiceKiosk.Api.Data;

/// <summary>
/// Applies pending migrations and creates one test account per role.
/// Runs at start-up in the Development environment only (see Program.cs).
/// All seeded details are fake (NFR-08) and the passwords come from User Secrets (NFR-06).
/// </summary>
public class DbSeeder
{
    public const string TestUserEmail = "user@kiosk.test";
    public const string TestSuperUserEmail = "superuser@kiosk.test";

    private readonly KioskDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly SeedSettings _seedSettings;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(
        KioskDbContext context,
        IPasswordHasher<User> passwordHasher,
        IOptions<SeedSettings> seedSettings,
        ILogger<DbSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _seedSettings = seedSettings.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_context.Database.GetMigrations().Any())
        {
            throw new InvalidOperationException(
                "No EF Core migrations were found. Create the first one with: "
                + "dotnet ef migrations add InitialCreate --project src/SelfServiceKiosk.Api --output-dir Data/Migrations");
        }

        await _context.Database.MigrateAsync(cancellationToken);

        await EnsureAccountAsync(
            firstName: "Test",
            surname: "User",
            email: TestUserEmail,
            mobileNumber: "0000000001",
            role: UserRole.User,
            password: _seedSettings.UserPassword,
            cancellationToken: cancellationToken);

        await EnsureAccountAsync(
            firstName: "Test",
            surname: "SuperUser",
            email: TestSuperUserEmail,
            mobileNumber: "0000000002",
            role: UserRole.SuperUser,
            password: _seedSettings.SuperUserPassword,
            cancellationToken: cancellationToken);
    }

    private async Task EnsureAccountAsync(
        string firstName,
        string surname,
        string email,
        string mobileNumber,
        UserRole role,
        string? password,
        CancellationToken cancellationToken)
    {
        var alreadyExists = await _context.Users.AnyAsync(user => user.Email == email, cancellationToken);
        if (alreadyExists)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning(
                "The {Role} test account was not created because its seed password is not configured. See README.md.",
                role);
            return;
        }

        var account = new User
        {
            FirstName = firstName,
            Surname = surname,
            Email = email,
            MobileNumber = mobileNumber,
            Role = role,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
        };
        account.PasswordHash = _passwordHasher.HashPassword(account, password);

        _context.Users.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created the {Role} test account.", role);
    }
}
