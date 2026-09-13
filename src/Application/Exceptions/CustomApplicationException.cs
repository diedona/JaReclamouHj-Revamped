namespace JaReclamouHoje.Application.Exceptions;

public class CustomApplicationException : Exception
{
    public int StatusCode { get; }

    public CustomApplicationException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }
}