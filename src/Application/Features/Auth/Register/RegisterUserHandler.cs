using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Application.Exceptions;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using MediatR;

namespace JaReclamouHoje.Application.Features.Auth.Register;

public class RegisterUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator
) : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (exists)
        {
            throw new EmailAlreadyInUseException(request.Email);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);
        return AuthResponse.Create(user, token, 3600);
    }
}