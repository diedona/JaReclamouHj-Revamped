using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Exceptions;
using JaReclamouHoje.Domain.ValueObjects;

namespace JaReclamouHoje.Domain.Services;

/// <summary>
/// Domain service for complaint cancellation. Exists because the authorization rule
/// (owner-or-admin) needs knowledge the <see cref="Complaint"/> aggregate does not have:
/// the actor's role. Keeps role/identity concerns out of the entity while keeping the
/// rule itself in the Domain (not Application).
/// Owns the clock: the service timestamps the cancellation via the injected
/// <see cref="TimeProvider"/>, so callers never pass timestamps around.
/// </summary>
public sealed class ComplaintCancellationService(TimeProvider timeProvider)
{
    private readonly TimeProvider _timeProvider = timeProvider;

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
    /// owner nor admin. Dull reason-shape validation lives in the
    /// <see cref="Entities.Validation.ComplaintCancellationValidator"/> (via the
    /// <see cref="ComplaintCancellationVO"/> validating factory) and surfaces as
    /// <see cref="FluentValidation.ValidationException"/>.
    /// The timestamp is stamped here from the injected clock and never validated:
    /// a future cancellation is unrepresentable by construction.
    /// </summary>
    public void Cancel(
        Complaint complaint,
        Guid actorUserId,
        bool actorIsAdmin,
        string reason
    )
    {
        ArgumentNullException.ThrowIfNull(complaint);

        if (complaint.IsCanceled)
            throw new ComplaintAlreadyCanceledException(complaint.Id);

        if (!CanCancel(complaint, actorUserId, actorIsAdmin))
            throw new ComplaintCancellationDeniedException(complaint.Id);

        complaint.Cancel(ComplaintCancellationVO.Create(actorUserId, reason, _timeProvider.GetUtcNow()));
    }
}
