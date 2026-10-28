using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Repositories;

/// <summary>The only place that queries the Users table.</summary>
public class UserRepository : IUserRepository
{
    private readonly KioskDbContext _context;

    public UserRepository(KioskDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalisedEmail = email.Trim().ToLowerInvariant();

        return await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == normalisedEmail, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalisedEmail = email.Trim().ToLowerInvariant();

        return await _context.Users.AnyAsync(user => user.Email == normalisedEmail, cancellationToken);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await SaveAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveAsync(cancellationToken);
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // The unique index on Email stopped a duplicate. The original message contains the email,
            // so it is not passed on or logged.
            throw new EmailAlreadyExistsException();
        }
    }
}
