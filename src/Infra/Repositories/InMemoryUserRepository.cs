using System.Collections.Concurrent;
using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Domain.Repositories;

namespace JaReclamouHoje.Infra.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public InMemoryUserRepository(IPasswordHasher passwordHasher)
    {
        SeedDefaultUser(passwordHasher);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = _users.Values.FirstOrDefault(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var exists = _users.Values.Any(x =>
            string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.TryAdd(user.Id, user);
        return Task.CompletedTask;
    }

    private void SeedDefaultUser(IPasswordHasher passwordHasher)
    {
        var passwordHash = passwordHasher.HashPassword("123123");
        var defaultUser = new User(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "diedona",
            "diedona@gmail.com",
            passwordHash,
            UserRoles.Admin,
            DateTime.UtcNow);

        _users.TryAdd(defaultUser.Id, defaultUser);
    }
}