namespace JaReclamouHoje.Application.Exceptions;

public class RoleAssignmentForbiddenException(string role)
    : CustomApplicationException(403, $"Only administrators can assign the '{role}' role.")
{
}
