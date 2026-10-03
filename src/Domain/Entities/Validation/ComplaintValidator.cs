using FluentValidation;

namespace JaReclamouHoje.Domain.Entities.Validation;

/// <summary>
/// Backstop input-shape validator for <see cref="Complaint"/>.
/// Mirrors the shape checks removed from the entity constructors.
/// Primary enforcement lives in Application command validators;
/// state-transition invariants (double-cancel) stay in <see cref="Complaint"/>.
/// </summary>
public sealed class ComplaintValidator : AbstractValidator<Complaint>
{
    public ComplaintValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Complaint id must not be empty.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Complaint title is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Complaint description is required.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty().WithMessage("Owner user id is required to create a complaint.");
    }
}
