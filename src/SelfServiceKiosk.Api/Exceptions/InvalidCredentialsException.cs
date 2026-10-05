namespace SelfServiceKiosk.Api.Exceptions;

/// <summary>
/// Thrown when a login attempt fails. The message is deliberately the same for an unknown
/// email, a wrong password and the wrong login endpoint, so an attacker learns nothing.
/// Translated to HTTP 401 by the ExceptionHandlingMiddleware.
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    {
    }
}
