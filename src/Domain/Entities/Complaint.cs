using JaReclamouHoje.Domain.Exceptions;

namespace JaReclamouHoje.Domain.Entities;

public sealed class Complaint
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public ComplaintCancellation? Cancellation { get; private set; }

    /// <summary>True once the complaint has been canceled (one-time terminal operation).</summary>
    public bool IsCanceled => Cancellation is not null;

    private Complaint()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    public Complaint(
        Guid id,
        string title,
        string description,
        DateTimeOffset createdAt,
        Guid createdByUserId = default,
        ComplaintCancellation? cancellation = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Complaint id must not be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Complaint title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Complaint description is required.", nameof(description));

        Id = id;
        Title = title.Trim();
        Description = description.Trim();
        CreatedAt = createdAt;
        CreatedByUserId = createdByUserId;
        Cancellation = cancellation;
    }

    public static Complaint Create(string title, string description, Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Owner user id is required to create a complaint.", nameof(createdByUserId));

        return new Complaint(
            Guid.NewGuid(),
            title,
            description,
            DateTimeOffset.UtcNow,
            createdByUserId
        );
    }

    /// <summary>
    /// Records the one-time terminal cancellation. Throws
    /// <see cref="ComplaintAlreadyCanceledException"/> when called twice.
    /// Authorization (owner-or-admin) is enforced by
    /// <see cref="Services.ComplaintCancellationService"/>, not here.
    /// </summary>
    public void Cancel(ComplaintCancellation cancellation)
    {
        ArgumentNullException.ThrowIfNull(cancellation);

        if (IsCanceled)
            throw new ComplaintAlreadyCanceledException(Id);

        Cancellation = cancellation;
    }

    /// <summary>Convenience overload that builds the <see cref="ComplaintCancellation"/> value object.</summary>
    public void Cancel(Guid canceledByUserId, string reason, DateTimeOffset canceledAt) =>
        Cancel(new ComplaintCancellation(canceledByUserId, reason, canceledAt));

    public bool IsOwnedBy(Guid userId) => userId != Guid.Empty && CreatedByUserId == userId;
}
