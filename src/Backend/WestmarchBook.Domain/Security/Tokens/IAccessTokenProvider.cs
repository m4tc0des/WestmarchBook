namespace WestmarchBook.Domain.Security.Tokens;

public interface IAccessTokenProvider
{
    string GetToken();
}
