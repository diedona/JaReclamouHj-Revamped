namespace JaReclamouHoje.Application.Exceptions;

public class EmailAlreadyInUseException : CustomApplicationException
{
    public EmailAlreadyInUseException(string email)
        : base(409, $"The email '{email}' is already registered.")
    {
    }
}