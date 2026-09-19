using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Application.Exceptions;
using JaReclamouHoje.Domain.Repositories;
using MediatR;

namespace JaReclamouHoje.Application.Features.Auth.Login;

public class LoginUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator
) : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return AuthResponse.Create(user, token, 3600);
    }
}