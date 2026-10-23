using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Data access for users. Services depend on this interface, not on EF Core.</summary>
public interface IUserRepository
{
    /// <summary>Finds an account by email address (case-insensitive), or returns null.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>True when an account with this email exists (case-insensitive).</summary>
Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

/// <summary>Saves a new account.</summary>
Task AddAsync(User user, CancellationToken cancellationToken = default);
}

