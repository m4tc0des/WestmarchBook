namespace WestmarchBook.Domain.Security.PasswordHashing;

public interface IPasswordHasher
{
    string PasswordHash(string password);

    bool VerifyPassword(string password, string passwordHash);
}
