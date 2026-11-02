using SelfServiceKiosk.Api.DTOs.Admin;
using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.DTOs.Common;

namespace SelfServiceKiosk.Api.Interfaces;

public interface IAdminUserService
{
    Task<PagedResult<AdminUserDto>> GetPageAsync(PageQuery query, string? search, CancellationToken cancellationToken = default);

    Task<AdminUserDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Deactivates or reactivates an account. Nothing is deleted, and an administrator cannot deactivate themself.</summary>
    Task<AdminUserDto> SetActiveAsync(int id, bool isActive, int currentUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates another Administrator (role SuperUser). Only an Administrator can call this.</summary>
    Task<AdminUserDto> CreateAdministratorAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
}
