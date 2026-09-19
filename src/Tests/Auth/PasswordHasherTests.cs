using JaReclamouHoje.Infra.Authentication;

namespace JaReclamouHoje.Tests.Auth;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_And_VerifyPassword_With_Correct_Password_Returns_True()
    {
        var hash = _hasher.HashPassword("123123");

        Assert.True(_hasher.VerifyPassword("123123", hash));
    }

    [Fact]
    public void VerifyPassword_With_Wrong_Password_Returns_False()
    {
        var hash = _hasher.HashPassword("123123");

        Assert.False(_hasher.VerifyPassword("wrong-password", hash));
    }

    [Fact]
    public void HashPassword_IsSalted_Produces_Different_Hashes_For_Same_Password()
    {
        var first = _hasher.HashPassword("123123");
        var second = _hasher.HashPassword("123123");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void VerifyPassword_With_Null_Or_Invalid_Format_Hash_Returns_False()
    {
        Assert.False(_hasher.VerifyPassword("123123", string.Empty));
        Assert.False(_hasher.VerifyPassword("123123", "not-a-valid-hash"));
    }
}