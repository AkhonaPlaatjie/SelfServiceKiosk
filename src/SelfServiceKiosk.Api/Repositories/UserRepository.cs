using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Exceptions;
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
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
{
    var normalisedEmail = email.Trim().ToLowerInvariant();

    return await _context.Users.AnyAsync(user => user.Email == normalisedEmail, cancellationToken);
}

public async Task AddAsync(User user, CancellationToken cancellationToken = default)
{
    _context.Users.Add(user);

    try
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
    {
        // Two people registered the same email at the same moment and the unique index stopped the second.
        // The original exception message contains the email, so it is not passed on or logged.
        throw new EmailAlreadyExistsException();
    }
}
}
