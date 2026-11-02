using Microsoft.AspNetCore.Identity;
using SelfServiceKiosk.Api.DTOs.Admin;
using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.DTOs.Common;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Mappers;
using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Services;

public class AdminUserService : IAdminUserService
{
    private const string NotFoundMessage = "User not found.";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AdminUserService> _logger;

    public AdminUserService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        ILogger<AdminUserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<PagedResult<AdminUserDto>> GetPageAsync(
        PageQuery query,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _userRepository.GetPageAsync(query.Page, query.PageSize, search, cancellationToken);

        return new PagedResult<AdminUserDto>
        {
            Items = items.Select(user => user.ToAdminDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total,
        };
    }

    public async Task<AdminUserDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        return user.ToAdminDto();
    }

    public async Task<AdminUserDto> SetActiveAsync(
        int id,
        bool isActive,
        int currentUserId,
        CancellationToken cancellationToken = default)
    {
        if (!isActive && id == currentUserId)
        {
            throw new ConflictException("You cannot deactivate your own account.");
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(NotFoundMessage);

        user.IsActive = isActive; // history (orders, transactions) is kept: nothing is deleted
        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Administrator {AdminId} set user {UserId} active state to {IsActive}.", currentUserId, id, isActive);

        return user.ToAdminDto();
    }

    public async Task<AdminUserDto> CreateAdministratorAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            // Never log the email address (NFR-09).
            _logger.LogWarning("Administrator creation refused: the email is already registered.");
            throw new EmailAlreadyExistsException();
        }

        var admin = new User
        {
            FirstName = request.FirstName.Trim(),
            Surname = request.Surname.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            MobileNumber = request.MobileNumber.Trim(),
            Role = UserRole.SuperUser,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
        };
        admin.PasswordHash = _passwordHasher.HashPassword(admin, request.Password);

        await _userRepository.AddAsync(admin, cancellationToken);

        _logger.LogInformation("Administrator account {UserId} created.", admin.Id);

        return admin.ToAdminDto();
    }
}
