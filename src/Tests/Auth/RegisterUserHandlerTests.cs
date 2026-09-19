using JaReclamouHoje.Application.Exceptions;
using JaReclamouHoje.Application.Features.Auth.Register;

namespace JaReclamouHoje.Tests.Auth;

public class RegisterUserHandlerTests
{
    [Fact]
    public async Task Register_Creates_User_And_Returns_Token()
    {
        var repository = new FakeUserRepository();
        var handler = new RegisterUserHandler(repository, new FakePasswordHasher(), new FakeJwtTokenGenerator());
        var command = new RegisterUserCommand("Diedona", "diedona@gmail.com", "123456");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("diedona@gmail.com", response.Email);
        Assert.Equal("Diedona", response.Name);
        Assert.Equal("token-for-diedona@gmail.com", response.Token);
        Assert.True(await repository.ExistsByEmailAsync("diedona@gmail.com"));
    }

    [Fact]
    public async Task Register_With_Duplicate_Email_Throws_EmailAlreadyInUseException()
    {
        var repository = new FakeUserRepository();
        var preExisting = Domain.Entities.User.Create("Already Here", "taken@email.com", "hashed:123456");
        await repository.AddAsync(preExisting);

        var handler = new RegisterUserHandler(repository, new FakePasswordHasher(), new FakeJwtTokenGenerator());
        var command = new RegisterUserCommand("New User", "TAKEN@email.com", "123456");

        var exception = await Assert.ThrowsAsync<EmailAlreadyInUseException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(409, exception.StatusCode);
    }
}