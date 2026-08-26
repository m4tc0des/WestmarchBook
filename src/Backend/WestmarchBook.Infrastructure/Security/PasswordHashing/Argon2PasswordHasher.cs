using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using WestmarchBook.Domain.Security.PasswordHashing;

namespace WestmarchBook.Infrastructure.Security.PasswordHashing;

internal class Argon2PasswordHasher : IPasswordHasher
{
    private const int DEGREE_OF_PARALLELISM = 1;
    private const int ITERATIONS = 1;
    private const int MEMORY_SIZE = 20 * 1024;
    private const int HASH_SIZE = 32;
    private const int SALT_SIZE = 16;
    public string PasswordHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);

        var hash = PasswordHash(password, salt);

        var combinedBytes = new byte[HASH_SIZE + SALT_SIZE];

        hash.CopyTo(combinedBytes);
        salt.CopyTo(combinedBytes, HASH_SIZE);

        return Convert.ToBase64String(combinedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var combinedBytes = Convert.FromBase64String(passwordHash);

        var hash = new byte[HASH_SIZE];
        var salt = new byte[SALT_SIZE];

        Array.Copy(combinedBytes, hash, HASH_SIZE);
        Array.Copy(combinedBytes, HASH_SIZE, salt, 0, SALT_SIZE);

        var newHash = PasswordHash(password, salt);

        return CryptographicOperations.FixedTimeEquals(newHash, hash);
    }

    private byte[] PasswordHash(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var hashAlgorithm = new Argon2id(passwordBytes)
        {
            DegreeOfParallelism = DEGREE_OF_PARALLELISM,
            Iterations = ITERATIONS,
            MemorySize = MEMORY_SIZE,
            Salt = salt,
        };
        return hashAlgorithm.GetBytes(HASH_SIZE);
    }
}
