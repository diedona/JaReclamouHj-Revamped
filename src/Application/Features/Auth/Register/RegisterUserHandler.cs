using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Application.Exceptions;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using MediatR;

namespace JaReclamouHoje.Application.Features.Auth.Register;

public class RegisterUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    ICurrentUser currentUser
) : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? UserRoles.User : request.Role.Trim();

        if (role == UserRoles.Admin && !_currentUser.IsInRole(UserRoles.Admin))
        {
            throw new RoleAssignmentForbiddenException(role);
        }

        var exists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (exists)
        {
            throw new EmailAlreadyInUseException(request.Email);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, passwordHash, role);

        await _userRepository.AddAsync(user, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);
        return AuthResponse.Create(user, token.Token, token.ExpiresInSeconds);
    }
}