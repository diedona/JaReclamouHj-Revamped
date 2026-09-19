using MediatR;

namespace JaReclamouHoje.Application.Features.Auth.Login;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<AuthResponse>;