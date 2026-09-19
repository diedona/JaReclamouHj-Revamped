using JaReclamouHoje.Application.Exceptions;
using JaReclamouHoje.Application.Features.Auth.Login;

namespace JaReclamouHoje.Tests.Auth;

public class LoginUserHandlerTests
{
    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_Token()
    {
        var repository = new FakeUserRepository();
        await repository.AddAsync(Domain.Entities.User.Create("Diedona", "diedona@gmail.com", "hashed:123123"));

        var handler = new LoginUserHandler(repository, new FakePasswordHasher(), new FakeJwtTokenGenerator());
        var command = new LoginUserCommand("diedona@gmail.com", "123123");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("diedona@gmail.com", response.Email);
        Assert.Equal("token-for-diedona@gmail.com", response.Token);
    }

    [Fact]
    public async Task Login_With_Wrong_Password_Throws_InvalidCredentialsException()
    {
        var repository = new FakeUserRepository();
        await repository.AddAsync(Domain.Entities.User.Create("Diedona", "diedona@gmail.com", "hashed:123123"));

        var handler = new LoginUserHandler(repository, new FakePasswordHasher(), new FakeJwtTokenGenerator());
        var command = new LoginUserCommand("diedona@gmail.com", "wrong-password");

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(401, exception.StatusCode);
    }

    [Fact]
    public async Task Login_With_Unknown_Email_Throws_InvalidCredentialsException()
    {
        var handler = new LoginUserHandler(new FakeUserRepository(), new FakePasswordHasher(), new FakeJwtTokenGenerator());
        var command = new LoginUserCommand("ghost@email.com", "123123");

        var exception = await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => handler.Handle(command, CancellationToken.None));

        Assert.Equal(401, exception.StatusCode);
    }
}