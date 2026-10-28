namespace SelfServiceKiosk.Api.Exceptions;

/// <summary>Thrown when the requested record does not exist (HTTP 404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
