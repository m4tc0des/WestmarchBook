using System.Security.Cryptography;
using WestmarchBook.Domain.Security.Tokens;

namespace WestmarchBook.Infrastructure.Security.Tokens.RefreshToken;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string Generate()
    {
        var token = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(token);
    }
}
