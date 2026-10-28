using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SelfServiceKiosk.Api.DTOs.Auth;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Exceptions;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;
using SelfServiceKiosk.Api.Services;

namespace SelfServiceKiosk.Api.Tests.Services;

/// <summary>
/// Unit tests for the login and registration rules. The repository and token service are mocked,
/// so these tests need no database and run in milliseconds.
/// Test names follow: Method_Scenario_ExpectedResult.
/// </summary>
public class AuthServiceTests
{
    private const string CorrectPassword = "Fake-Test-Passw0rd!";
    private static readonly DateTime TokenExpiry = new(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _tokenService
            .Setup(service => service.CreateAccessToken(It.IsAny<User>()))
            .Returns(new TokenResult("test-token", TokenExpiry));

        _sut = new AuthService(
            _userRepository.Object,
            _passwordHasher,
            _tokenService.Object,
            NullLogger<AuthService>.Instance);
    }

    [Fact]
    public async Task LoginAsync_UserWithCorrectPassword_ReturnsTokenAndUserDetails()
    {
        var account = ArrangeExistingAccount(UserRole.User);
        var request = new LoginRequestDto { Email = account.Email, Password = CorrectPassword };

        var response = await _sut.LoginAsync(request, UserRole.User);

        Assert.Equal("test-token", response.AccessToken);
        Assert.Equal(TokenExpiry, response.ExpiresAtUtc);
        Assert.Equal(account.Id, response.User.Id);
        Assert.Equal(UserRole.User, response.User.Role);
    }

    [Fact]
    public async Task LoginAsync_SuperUserWithCorrectPassword_ReturnsSuperUserRole()
    {
        var account = ArrangeExistingAccount(UserRole.SuperUser);
        var request = new LoginRequestDto { Email = account.Email, Password = CorrectPassword };

        var response = await _sut.LoginAsync(request, UserRole.SuperUser);

        Assert.Equal(UserRole.SuperUser, response.User.Role);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ThrowsInvalidCredentials()
    {
        _userRepository
            .Setup(repository => repository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var request = new LoginRequestDto { Email = "nobody@kiosk.test", Password = CorrectPassword };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request, UserRole.User));

        AssertNoTokenWasIssued();
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsInvalidCredentials()
    {
        var account = ArrangeExistingAccount(UserRole.User);
        var request = new LoginRequestDto { Email = account.Email, Password = "Wrong-Passw0rd!" };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request, UserRole.User));

        AssertNoTokenWasIssued();
    }

    [Fact]
    public async Task LoginAsync_UserOnSuperUserEndpoint_ThrowsInvalidCredentials()
    {
        var account = ArrangeExistingAccount(UserRole.User);
        var request = new LoginRequestDto { Email = account.Email, Password = CorrectPassword };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request, UserRole.SuperUser));

        AssertNoTokenWasIssued();
    }

    [Fact]
    public async Task LoginAsync_SuperUserOnUserEndpoint_ThrowsInvalidCredentials()
    {
        var account = ArrangeExistingAccount(UserRole.SuperUser);
        var request = new LoginRequestDto { Email = account.Email, Password = CorrectPassword };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request, UserRole.User));

        AssertNoTokenWasIssued();
    }

    // BR-09: a deactivated user cannot log in.
    [Fact]
    public async Task LoginAsync_DeactivatedAccount_ThrowsAccountDeactivated()
    {
        var account = ArrangeExistingAccount(UserRole.User, isActive: false);
        var request = new LoginRequestDto { Email = account.Email, Password = CorrectPassword };

        await Assert.ThrowsAsync<AccountDeactivatedException>(() => _sut.LoginAsync(request, UserRole.User));

        AssertNoTokenWasIssued();
    }

    // A deactivated account with the WRONG password must look like any other failed login,
    // otherwise the endpoint would reveal which emails exist.
    [Fact]
    public async Task LoginAsync_DeactivatedAccountWithWrongPassword_ThrowsInvalidCredentials()
    {
        var account = ArrangeExistingAccount(UserRole.User, isActive: false);
        var request = new LoginRequestDto { Email = account.Email, Password = "Wrong-Passw0rd!" };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _sut.LoginAsync(request, UserRole.User));
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_CreatesUserWithUserRoleAndHashedPassword()
    {
        User? saved = null;
        _userRepository
            .Setup(repository => repository.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepository
            .Setup(repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => saved = user)
            .Returns(Task.CompletedTask);
        var request = new RegisterRequestDto
        {
            FirstName = " Thandi ",
            Surname = "Nkosi",
            Email = " Thandi.Nkosi@Kiosk.Test ",
            MobileNumber = "0821234567",
            Password = CorrectPassword,
        };

        var response = await _sut.RegisterAsync(request);

        var created = Assert.IsType<User>(saved);
        Assert.Equal(UserRole.User, created.Role);
        Assert.Equal("thandi.nkosi@kiosk.test", created.Email);
        Assert.Equal("Thandi", created.FirstName);
        Assert.NotEqual(CorrectPassword, created.PasswordHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            _passwordHasher.VerifyHashedPassword(created, created.PasswordHash, CorrectPassword));
        Assert.Equal(UserRole.User, response.Role);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyRegistered_ThrowsAndSavesNothing()
    {
        _userRepository
            .Setup(repository => repository.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var request = new RegisterRequestDto
        {
            FirstName = "Thandi",
            Surname = "Nkosi",
            Email = "thandi.nkosi@kiosk.test",
            MobileNumber = "0821234567",
            Password = CorrectPassword,
        };

        await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _sut.RegisterAsync(request));

        _userRepository.Verify(
            repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void RegisterRequestDto_HasNoRoleProperty_SoNobodyCanRegisterAsAdministrator()
    {
        var propertyNames = typeof(RegisterRequestDto).GetProperties().Select(property => property.Name);

        Assert.DoesNotContain("Role", propertyNames);
    }

    private User ArrangeExistingAccount(UserRole role, bool isActive = true)
    {
        var account = TestUsers.Create(role, isActive);
        account.PasswordHash = _passwordHasher.HashPassword(account, CorrectPassword);

        _userRepository
            .Setup(repository => repository.GetByEmailAsync(account.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        return account;
    }

    private void AssertNoTokenWasIssued()
    {
        _tokenService.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }
}
