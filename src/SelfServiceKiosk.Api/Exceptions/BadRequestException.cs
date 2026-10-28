namespace SelfServiceKiosk.Api.Exceptions;

/// <summary>Thrown when the requested record does not exist (HTTP 400).</summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message)
        : base(message)
    {
    }
}
