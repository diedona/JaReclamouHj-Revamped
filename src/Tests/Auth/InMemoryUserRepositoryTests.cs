using JaReclamouHoje.Domain.Entities;
using JaReclamouHoje.Infra.Authentication;
using JaReclamouHoje.Infra.Repositories;

namespace JaReclamouHoje.Tests.Auth;

public class InMemoryUserRepositoryTests
{
    [Fact]
    public async Task Default_User_Is_Seeded_With_Password_123123()
    {
        var repository = new InMemoryUserRepository();

        var user = await repository.GetByEmailAsync("diedona@gmail.com");

        Assert.NotNull(user);
        Assert.Equal("diedona", user.Name);
        Assert.Equal(UserRoles.Admin, user.Role);
        Assert.True(new PasswordHasher().VerifyPassword("123123", user.PasswordHash));
    }

    [Fact]
    public async Task GetByEmail_Is_CaseInsensitive()
    {
        var repository = new InMemoryUserRepository();

        var user = await repository.GetByEmailAsync("DIEDONA@GMAIL.COM");

        Assert.NotNull(user);
    }

    [Fact]
    public async Task AddAsync_Makes_User_Retrievable()
    {
        var repository = new InMemoryUserRepository();
        var newUser = Domain.Entities.User.Create("Tester", "tester@test.com", "hashed:password");

        await repository.AddAsync(newUser);

        var user = await repository.GetByEmailAsync("tester@test.com");
        Assert.NotNull(user);
        Assert.Equal("Tester", user.Name);
    }
}