using FluentValidation;
using JaReclamouHoje.Domain.Common.Validation;
using JaReclamouHoje.Domain.ValueObjects;

namespace JaReclamouHoje.Domain.Entities.Validation;

/// <summary>
/// Dull backstop input-shape validator for <see cref="ComplaintCancellationVO"/>:
/// user id present, reason present and within length. No temporal or
/// business logic here: the service stamps cancellations itself, so a
/// future timestamp is unrepresentable by construction.
/// Primary enforcement lives in Application command validators (HTTP 400s);
/// this validator guards direct Domain construction (factories, services).
/// State-transition invariants (double-cancel, owner-or-admin) are NOT here:
/// they live in <see cref="Complaint"/> and <see cref="Services.ComplaintCancellationService"/>.
/// </summary>
public sealed class ComplaintCancellationValidator : AbstractValidator<ComplaintCancellationVO>
{
    public ComplaintCancellationValidator()
    {
        RuleFor(x => x.CanceledByUserId)
            .NotEmpty().WithMessage("Canceling user id must not be empty.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required.")
            .Must(reason => !string.IsNullOrWhiteSpace(reason)
                && reason.Trim().Length <= CancellationRules.MaxReasonLength)
            .WithMessage($"Cancellation reason must not exceed {CancellationRules.MaxReasonLength} characters.");
    }
}
