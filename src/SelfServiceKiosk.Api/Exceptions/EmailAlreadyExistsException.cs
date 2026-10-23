namespace SelfServiceKiosk.Api.Exceptions;

/// <summary>Thrown when someone registers with an email that is already in use.</summary>
public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException()
        : base("An account with this email address already exists.")
    {
        EmailAlreadyExistsException => (StatusCodes.Status409Conflict, exception.Message);
    }
}
