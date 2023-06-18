using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Common.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool IsPasswordValid(string password, string passwordHash);
}

internal class PasswordHasher : IPasswordHasher
{
    private const int iterCount = 10000;
    private const int saltLength = 128 / 8;
    private const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
    private const int numBytesRequested = 256 / 8;

    private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    /// <summary>
    /// Get password hash (salt + subkey).
    /// </summary>
    /// <returns>The calculated hash (48 bytes)</returns>
    public string HashPassword(string password)
    {
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        byte[] salt = new byte[saltLength];
        rng.GetBytes(salt);

        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

        var outputBytes = new byte[saltLength + subkey.Length];
        Buffer.BlockCopy(salt, 0, outputBytes, 0, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, saltLength, subkey.Length);

        return Convert.ToBase64String(outputBytes);
    }

    public bool IsPasswordValid(string password, string passwordHash)
    {
        byte[] hash = Convert.FromBase64String(passwordHash);

        if (hash == null)
        {
            throw new ArgumentNullException(nameof(hash));
        }
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        if (hash.Length != saltLength + numBytesRequested)
        {
            return false;
        }

        try
        {
            byte[] salt = new byte[saltLength];
            Buffer.BlockCopy(hash, 0, salt, 0, saltLength);

            int subkeyLength = hash.Length - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }

            byte[] expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(hash, salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
            return actualSubkey.SequenceEqual(expectedSubkey);
        }
        catch
        {
            return false;
        }
    }
}
