namespace SelfServiceKiosk.Api.Exceptions;

/// <summary>Thrown when the requested record does not exist (HTTP 409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
