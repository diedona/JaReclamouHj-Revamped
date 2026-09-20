using FluentValidation;
using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Application.Features.Auth.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Role)
            .Must(role => string.IsNullOrWhiteSpace(role) || role.Trim() == UserRoles.User || role.Trim() == UserRoles.Admin)
            .WithMessage("Role must be 'User' or 'Admin'.");
    }
}