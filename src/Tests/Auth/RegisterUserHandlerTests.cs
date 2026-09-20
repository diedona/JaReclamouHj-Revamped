using JaReclamouHoje.Application.Common.Interfaces;
using JaReclamouHoje.Application.Exceptions;
using JaReclamouHoje.Application.Features.Auth.Register;
using JaReclamouHoje.Domain.Entities;

namespace JaReclamouHoje.Tests.Auth;

public class RegisterUserHandlerTests
{
    private static RegisterUserHandler CreateHandler(FakeUserRepository repository, ICurrentUser currentUser) =>
        new(repository, new FakePasswordHasher(), new FakeJwtTokenGenerator(), currentUser);

    [Fact]
    public async Task Register_Creates_User_And_Returns_Token()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler(repository, new FakeCurrentUser());
        var command = new RegisterUserCommand("Diedona", "diedona@gmail.com", "123456");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("diedona@gmail.com", response.Email);
        Assert.Equal("Diedona", response.Name);
        Assert.Equal(UserRoles.User, response.Role);
        Assert.Equal("token-for-diedona@gmail.com", response.Token);
        Assert.True(await repository.ExistsByEmailAsync("diedona@gmail.com"));
    }

    [Fact]
    public async Task Register_With_Duplicate_Email_Throws_EmailAlreadyInUseException()
    {
        var repository = new FakeUserRepository();
        var preExisting = Domain.Entities.User.Create("Already Here", "taken@email.com", "hashed:123456");
        await repository.AddAsync(preExisting);

        var handler = CreateHandler(repository, new FakeCurrentUser());
        var command = new RegisterUserCommand("New User", "TAKEN@email.com", "123456");

        var exception = await Assert.ThrowsAsync<EmailAlreadyInUseException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(409, exception.StatusCode);
    }

    [Fact]
    public async Task Register_Admin_By_Anonymous_Throws_RoleAssignmentForbiddenException()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler(repository, new FakeCurrentUser());
        var command = new RegisterUserCommand("Eve", "eve@test.com", "123456", UserRoles.Admin);

        var exception = await Assert.ThrowsAsync<RoleAssignmentForbiddenException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(403, exception.StatusCode);
        Assert.False(await repository.ExistsByEmailAsync("eve@test.com"));
    }

    [Fact]
    public async Task Register_Admin_By_NonAdmin_Throws_RoleAssignmentForbiddenException()
    {
        var repository = new FakeUserRepository();
        var caller = new FakeCurrentUser(isAuthenticated: true, userId: Guid.NewGuid(), role: UserRoles.User);
        var handler = CreateHandler(repository, caller);
        var command = new RegisterUserCommand("Eve", "eve@test.com", "123456", UserRoles.Admin);

        var exception = await Assert.ThrowsAsync<RoleAssignmentForbiddenException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(403, exception.StatusCode);
    }

    [Fact]
    public async Task Register_Admin_By_Admin_Succeeds()
    {
        var repository = new FakeUserRepository();
        var caller = new FakeCurrentUser(isAuthenticated: true, userId: Guid.NewGuid(), role: UserRoles.Admin);
        var handler = CreateHandler(repository, caller);
        var command = new RegisterUserCommand("Root", "root@test.com", "123456", UserRoles.Admin);

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(UserRoles.Admin, response.Role);
        Assert.True(await repository.ExistsByEmailAsync("root@test.com"));
    }

    [Fact]
    public async Task Register_Denies_Admin_Before_Checking_Email()
    {
        var repository = new FakeUserRepository();
        var preExisting = User.Create("Already Here", "taken@email.com", "hashed:123456");
        await repository.AddAsync(preExisting);

        var handler = CreateHandler(repository, new FakeCurrentUser());
        var command = new RegisterUserCommand("Eve", "taken@email.com", "123456", UserRoles.Admin);

        await Assert.ThrowsAsync<RoleAssignmentForbiddenException>(
            () => handler.Handle(command, CancellationToken.None));
    }
}
