namespace JaReclamouHoje.Application.Exceptions;

public class InvalidCredentialsException : CustomApplicationException
{
    public InvalidCredentialsException()
        : base(401, "Invalid email or password.")
    {
    }
}