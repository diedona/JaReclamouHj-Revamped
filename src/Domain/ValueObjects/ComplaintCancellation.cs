using FluentValidation;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Entities.Validation;

namespace JaReclamouHoje.Domain.ValueObjects;

/// <summary>
/// Value object recording the single terminal cancellation of a <see cref="Complaint"/>.
/// Owned by the Complaint aggregate: no identity, no repository, no independent lifecycle.
/// A complaint is canceled if and only if <see cref="Complaint.Cancellation"/> is not null.
/// Input-shape validation lives in <see cref="ComplaintCancellationValidator"/>
/// (Domain backstop) and in Application command validators (primary gate);
/// the constructor only normalizes (trims) and assigns.
/// </summary>
public sealed record ComplaintCancellationVO
{
    public Guid CanceledByUserId { get; }
    public string Reason { get; }
    public DateTimeOffset CanceledAt { get; }

    public ComplaintCancellationVO(Guid canceledByUserId, string reason, DateTimeOffset canceledAt)
    {
        CanceledByUserId = canceledByUserId;
        Reason = (reason ?? string.Empty).Trim();
        CanceledAt = canceledAt;
    }

    /// <summary>
    /// Validating factory for dull input shape (user id, reason length).
    /// Throws <see cref="ValidationException"/> on invalid input.
    /// The timestamp is carried, not validated: the service stamps it,
    /// so a future cancellation is unrepresentable by construction.
    /// Prefer over the raw constructor on all Domain paths.
    /// </summary>
    public static ComplaintCancellationVO Create(
        Guid canceledByUserId,
        string reason,
        DateTimeOffset canceledAt)
    {
        var cancellation = new ComplaintCancellationVO(canceledByUserId, reason, canceledAt);
        new ComplaintCancellationValidator().ValidateAndThrow(cancellation);
        return cancellation;
    }
}
