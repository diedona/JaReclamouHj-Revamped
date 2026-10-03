using JaReclamouHoje.Domain.Exceptions.Base;
using System.Net;

namespace JaReclamouHoje.Domain.Exceptions;

/// <summary>Thrown when attempting to cancel a complaint that is already canceled.</summary>
public sealed class ComplaintAlreadyCanceledException : DomainException
{
    private static readonly HttpStatusCode statusCode = HttpStatusCode.Conflict;

    public ComplaintAlreadyCanceledException()
        : base("The complaint is already canceled.", (int)statusCode)
    {
    }

    public ComplaintAlreadyCanceledException(Guid complaintId)
        : base($"The complaint '{complaintId}' is already canceled.", (int)statusCode)
    {
    }
}
