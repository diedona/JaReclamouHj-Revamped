using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Exceptions;

namespace JaReclamouHoje.Domain.Services;

/// <summary>
/// Domain service for complaint cancellation. Exists because the authorization rule
/// (owner-or-admin) needs knowledge the <see cref="Complaint"/> aggregate does not have:
/// the actor's role. Keeps role/identity concerns out of the entity while keeping the
/// rule itself in the Domain (not Application).
/// </summary>
public sealed class ComplaintCancellationService
{
    /// <summary>
    /// Whether <paramref name="actorUserId"/> may cancel <paramref name="complaint"/>:
    /// not already canceled, and actor is admin or the complaint owner.
    /// </summary>
    public bool CanCancel(Complaint complaint, Guid actorUserId, bool actorIsAdmin)
    {
        ArgumentNullException.ThrowIfNull(complaint);

        if (complaint.IsCanceled)
            return false;

        return actorIsAdmin || complaint.IsOwnedBy(actorUserId);
    }

    /// <summary>
    /// Cancels the complaint, recording when/who/why. Throws
    /// <see cref="ComplaintAlreadyCanceledException"/> if already canceled, or
    /// <see cref="ComplaintCancellationDeniedException"/> when the actor is neither
    /// owner nor admin. Reason/timestamp validation lives in
    /// <see cref="ComplaintCancellation"/> and surfaces as <see cref="ArgumentException"/>.
    /// </summary>
    public void Cancel(
        Complaint complaint,
        Guid actorUserId,
        bool actorIsAdmin,
        string reason,
        DateTimeOffset canceledAt)
    {
        ArgumentNullException.ThrowIfNull(complaint);

        if (complaint.IsCanceled)
            throw new ComplaintAlreadyCanceledException(complaint.Id);

        if (!CanCancel(complaint, actorUserId, actorIsAdmin))
            throw new ComplaintCancellationDeniedException(complaint.Id);

        complaint.Cancel(new ComplaintCancellation(actorUserId, reason, canceledAt));
    }
}
