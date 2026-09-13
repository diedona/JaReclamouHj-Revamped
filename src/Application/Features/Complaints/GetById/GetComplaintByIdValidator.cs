using FluentValidation;

namespace JaReclamouHoje.Application.Features.Complaints.GetById;

public class GetComplaintByIdValidator : AbstractValidator<GetComplaintByIdQuery>
{
    public GetComplaintByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("IdCantBeNull");
    }
}
