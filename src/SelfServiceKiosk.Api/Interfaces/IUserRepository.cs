using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Data access for users. Services depend on this interface, not on EF Core.</summary>
public interface IUserRepository
{
    /// <summary>Finds an account by email address (case-insensitive), or returns null.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>True when an account with this email exists (case-insensitive).</summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Finds an account by id for changing it (tracked), or returns null.</summary>
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>One page of accounts, optionally filtered by a name or email prefix.</summary>
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPageAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
