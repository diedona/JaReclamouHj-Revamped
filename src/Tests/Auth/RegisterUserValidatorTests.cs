using JaReclamouHoje.Application.Features.Auth.Register;

namespace JaReclamouHoje.Tests.Auth;

public class RegisterUserValidatorTests
{
    private readonly RegisterUserValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("  User  ")]
    public void Valid_Roles_Pass(string? role)
    {
        var command = new RegisterUserCommand("Name", "valid@test.com", "123456", role);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("admin")]
    [InlineData("ADMIN")]
    public void Unknown_Roles_Fail(string? role)
    {
        var command = new RegisterUserCommand("Name", "valid@test.com", "123456", role);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterUserCommand.Role));
    }
}
