using FluentValidation;
using JaReclamouHoje.Domain.Exceptions;
using JaReclamouHoje.Domain.Entities.Validation;
using JaReclamouHoje.Domain.ValueObjects;

namespace JaReclamouHoje.Domain.Entities;

public sealed class Complaint
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public ComplaintCancellationVO? Cancellation { get; private set; }

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
        Guid createdByUserId,
        ComplaintCancellationVO? cancellation)
    {
        // Dumb constructor: normalization only. Input-shape validation lives in
        // ComplaintValidator (Domain backstop) and Application command validators.
        Id = id;
        Title = (title ?? string.Empty).Trim();
        Description = (description ?? string.Empty).Trim();
        CreatedAt = createdAt;
        CreatedByUserId = createdByUserId;
        Cancellation = cancellation;
    }

    /// <summary>
    /// Validating factory. Throws <see cref="ValidationException"/> on invalid input.
    /// Prefer over the raw constructor on all Domain paths.
    /// </summary>
    public static Complaint Create(
        string title,
        string description,
        Guid createdByUserId,
        TimeProvider? timeProvider = null)
    {
        var clock = timeProvider ?? TimeProvider.System;

        var complaint = new Complaint(
            Guid.NewGuid(),
            title,
            description,
            clock.GetUtcNow(),
            createdByUserId,
            cancellation: null
        );

        new ComplaintValidator().ValidateAndThrow(complaint);
        return complaint;
    }

    /// <summary>
    /// Records the one-time terminal cancellation. Throws
    /// <see cref="ComplaintAlreadyCanceledException"/> when called twice.
    /// Authorization (owner-or-admin) is enforced by
    /// <see cref="Services.ComplaintCancellationService"/>, not here.
    /// </summary>
    public void Cancel(ComplaintCancellationVO cancellation)
    {
        ArgumentNullException.ThrowIfNull(cancellation);

        if (IsCanceled)
            throw new ComplaintAlreadyCanceledException(Id);

        Cancellation = cancellation;
    }

    public bool IsOwnedBy(Guid userId) => userId != Guid.Empty && CreatedByUserId == userId;
}
