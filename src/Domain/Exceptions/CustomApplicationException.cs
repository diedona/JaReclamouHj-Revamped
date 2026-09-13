using System.Net;

namespace JaReclamouHoje.Domain.Exceptions;

public class CustomApplicationException : Exception
{
    public readonly HttpStatusCode StatusCode;

    public CustomApplicationException(
        HttpStatusCode statusCode, 
        string message
    ) : base(message) 
    { 
        StatusCode = statusCode;
    }
}
