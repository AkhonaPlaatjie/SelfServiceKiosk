using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;
using SelfServiceKiosk.Api.Settings;

namespace SelfServiceKiosk.Api.Services;

/// <summary>Builds and signs the JWT that a client sends back on every later request.</summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenResult CreateAccessToken(User user)
    {
        var issuedAtUtc = DateTime.UtcNow;
        var expiresAtUtc = issuedAtUtc.AddMinutes(_settings.ExpiryMinutes);

        // Keep the token small and free of personal information (POPIA): an Id and a role are enough.
        var claims = new List<Claim>
        {
            new(KioskClaimTypes.UserId, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(KioskClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResult(tokenText, expiresAtUtc);
    }
}
