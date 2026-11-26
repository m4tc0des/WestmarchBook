namespace WestmarchBook.Domain.Security.Tokens;

public interface IRefreshTokenGenerator
{
    string Generate();
}
