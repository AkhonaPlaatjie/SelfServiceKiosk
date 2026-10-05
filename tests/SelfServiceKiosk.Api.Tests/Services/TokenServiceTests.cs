using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.Enums;
using SelfServiceKiosk.Api.Services;
using SelfServiceKiosk.Api.Settings;

namespace SelfServiceKiosk.Api.Tests.Services;

public class TokenServiceTests
{
    private readonly JwtSettings _settings = new()
    {
        Issuer = "test-issuer",
        Audience = "test-audience",
        Key = "fake-unit-test-signing-key-0123456789",
        ExpiryMinutes = 15,
    };

    private readonly TokenService _sut;

    public TokenServiceTests()
    {
        _sut = new TokenService(Options.Create(_settings));
    }

    [Fact]
    public void CreateAccessToken_SuperUser_WritesIdAndRoleClaims()
    {
        var account = TestUsers.Create(UserRole.SuperUser, id: 42);

        var result = _sut.CreateAccessToken(account);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.Equal("42", jwt.Claims.Single(claim => claim.Type == KioskClaimTypes.UserId).Value);
        Assert.Equal("SuperUser", jwt.Claims.Single(claim => claim.Type == KioskClaimTypes.Role).Value);
    }

    [Fact]
    public void CreateAccessToken_AnyAccount_UsesConfiguredIssuerAndAudience()
    {
        var result = _sut.CreateAccessToken(TestUsers.Create());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.Equal("test-issuer", jwt.Issuer);
        Assert.Contains("test-audience", jwt.Audiences);
    }

    [Fact]
    public void CreateAccessToken_AnyAccount_ExpiresAfterConfiguredMinutes()
    {
        var before = DateTime.UtcNow;

        var result = _sut.CreateAccessToken(TestUsers.Create());

        var after = DateTime.UtcNow;
        Assert.InRange(result.ExpiresAtUtc, before.AddMinutes(15), after.AddMinutes(15));
    }

    // POPIA: the token is readable by anyone who holds it, so it must not carry personal information.
    [Fact]
    public void CreateAccessToken_AnyAccount_ContainsNoPersonalInformation()
    {
        var account = TestUsers.Create();

        var result = _sut.CreateAccessToken(account);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.DoesNotContain(jwt.Claims, claim => claim.Value == account.Email);
        Assert.DoesNotContain(jwt.Claims, claim => claim.Value == account.MobileNumber);
        Assert.DoesNotContain(jwt.Claims, claim => claim.Value == account.Surname);
    }
}
