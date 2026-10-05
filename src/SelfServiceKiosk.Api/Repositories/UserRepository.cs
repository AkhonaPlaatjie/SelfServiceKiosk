using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Repositories;

/// <summary>
/// The only place that queries the Users table. Repositories contain no business rules:
/// they fetch and save, and the services decide what that data means.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly KioskDbContext _context;

    public UserRepository(KioskDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // Emails are stored trimmed and lower-case, so normalise the input the same way.
        var normalisedEmail = email.Trim().ToLowerInvariant();

        // AsNoTracking: this is a read-only query, so EF Core does not need to track the entity.
        return await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == normalisedEmail, cancellationToken);
    }
}
