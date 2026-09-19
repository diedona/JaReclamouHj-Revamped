using MediatR;

namespace JaReclamouHoje.Application.Features.Auth.Register;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password,
    string? Role = null
) : IRequest<AuthResponse>;