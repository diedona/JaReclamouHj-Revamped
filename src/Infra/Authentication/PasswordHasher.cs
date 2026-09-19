using System.Security.Cryptography;
using JaReclamouHoje.Application.Common.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace JaReclamouHoje.Infra.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8; // 16 bytes
    private const int NumBytesRequested = 256 / 8; // 32 bytes
    private const int IterationCount = 100_000;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var subkey = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: NumBytesRequested);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(subkey)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var parts = passwordHash.Split('.');
        if (parts.Length != 2)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(parts[0]);
            var expectedSubkey = Convert.FromBase64String(parts[1]);

            var actualSubkey = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: NumBytesRequested);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}