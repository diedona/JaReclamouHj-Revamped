using FluentValidation;
using JaReclamouHoje.Domain.Common.Validation;

namespace JaReclamouHoje.Application.Features.Complaints.Cancel;

public class CancelComplaintValidator : AbstractValidator<CancelComplaintCommand>
{
    public CancelComplaintValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required.")
            .Must(reason => !string.IsNullOrWhiteSpace(reason)
                && reason.Trim().Length <= CancellationRules.MaxReasonLength)
            .WithMessage($"Cancellation reason must not exceed {CancellationRules.MaxReasonLength} characters.");
    }
}
