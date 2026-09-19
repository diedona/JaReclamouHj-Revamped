using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;
using static JaReclamouHoje.Application.Common.Interfaces.IJwtTokenGenerator;

namespace JaReclamouHoje.Tests;

public class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => $"hashed:{password}";

    public bool VerifyPassword(string password, string passwordHash) =>
        passwordHash == $"hashed:{password}";
}

public class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public JwtTokenResult GenerateToken(User user) => new($"token-for-{user.Email}", 3600);
}

public class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<string, User> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_usersByEmail.Values.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        _usersByEmail.TryGetValue(email, out var user);
        return Task.FromResult(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(_usersByEmail.ContainsKey(email));

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _usersByEmail[user.Email] = user;
        return Task.CompletedTask;
    }
}