using WestmarchBook.Domain.Security.Tokens;

namespace WestmarchBook.Desktop.Services;

public class DesktopTokenProvider : IAccessTokenProvider
{
    public string Token { get; set; } = string.Empty;

    public string GetToken() => Token;
}
