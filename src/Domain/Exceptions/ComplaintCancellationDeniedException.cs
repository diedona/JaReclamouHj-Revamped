using JaReclamouHoje.Domain.Exceptions.Base;
using System.Net;

namespace JaReclamouHoje.Domain.Exceptions;

/// <summary>Thrown when an actor is not allowed to cancel a complaint (neither owner nor admin).</summary>
public sealed class ComplaintCancellationDeniedException : DomainException
{
    public ComplaintCancellationDeniedException()
        : base("Only the complaint owner or an admin can cancel this complaint.", (int)HttpStatusCode.Forbidden)
    {
    }

    public ComplaintCancellationDeniedException(Guid complaintId)
        : base($"Only the complaint owner or an admin can cancel the complaint '{complaintId}'.", (int)HttpStatusCode.Forbidden)
    {
    }
}
