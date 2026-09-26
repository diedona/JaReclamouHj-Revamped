namespace JaReclamouHoje.Domain.Entities;

/// <summary>
/// Value object recording the single terminal cancellation of a <see cref="Complaint"/>.
/// Owned by the Complaint aggregate: no identity, no repository, no independent lifecycle.
/// A complaint is canceled if and only if <see cref="Complaint.Cancellation"/> is not null.
/// </summary>
public sealed record ComplaintCancellation
{
    /// <summary>Maximum length of the cancellation reason.</summary>
    public const int MaxReasonLength = 500;

    /// <summary>Clock-skew tolerance for future <see cref="CanceledAt"/> values.</summary>
    public static readonly TimeSpan FutureTolerance = TimeSpan.FromMinutes(1);

    public Guid CanceledByUserId { get; }
    public string Reason { get; }
    public DateTimeOffset CanceledAt { get; }

    public ComplaintCancellation(Guid canceledByUserId, string reason, DateTimeOffset canceledAt)
    {
        if (canceledByUserId == Guid.Empty)
            throw new ArgumentException("Canceling user id must not be empty.", nameof(canceledByUserId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));

        var trimmedReason = reason.Trim();

        if (trimmedReason.Length > MaxReasonLength)
            throw new ArgumentOutOfRangeException(
                nameof(reason),
                $"Cancellation reason must not exceed {MaxReasonLength} characters.");

        if (canceledAt > DateTimeOffset.UtcNow + FutureTolerance)
            throw new ArgumentOutOfRangeException(
                nameof(canceledAt),
                "Cancellation timestamp must not be in the future.");

        CanceledByUserId = canceledByUserId;
        Reason = trimmedReason;
        CanceledAt = canceledAt;
    }

    public static ComplaintCancellation Create(Guid canceledByUserId, string reason, DateTimeOffset canceledAt) =>
        new(canceledByUserId, reason, canceledAt);
}
