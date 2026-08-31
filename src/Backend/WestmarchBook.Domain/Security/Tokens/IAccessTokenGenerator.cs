using WestmarchBook.Domain.Entities;

namespace WestmarchBook.Domain.Security.Tokens;

public interface IAccessTokenGenerator
{
    string Generate(User user);
}
