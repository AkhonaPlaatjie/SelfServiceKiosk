namespace SelfServiceKiosk.Api.Exceptions;

/// <summary>
/// Thrown when a deactivated account supplies correct credentials (BR-09).
/// Translated to HTTP 403 by the ExceptionHandlingMiddleware.
/// </summary>
public class AccountDeactivatedException : Exception
{
    public AccountDeactivatedException()
        : base("This account has been deactivated. Please contact an administrator.")
    {
    }
}
