using SelfServiceKiosk.Api.Models;

namespace SelfServiceKiosk.Api.Interfaces;

/// <summary>Creates signed JWT access tokens.</summary>
public interface ITokenService
{
    TokenResult CreateAccessToken(User user);
}
