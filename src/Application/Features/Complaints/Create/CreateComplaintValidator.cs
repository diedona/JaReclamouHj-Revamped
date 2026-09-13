using FluentValidation;

namespace JaReclamouHoje.Application.Features.Complaints.Create;

public class CreateComplaintValidator : AbstractValidator<CreateComplaintCommand>
{
    public CreateComplaintValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(2);
    }
}
