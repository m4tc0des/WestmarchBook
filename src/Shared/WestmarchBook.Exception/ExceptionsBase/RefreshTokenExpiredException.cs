using System.Net;

namespace WestmarchBook.Exception.ExceptionsBase;

public class RefreshTokenExpiredException : WestmarchBookException
{
    public override List<string> GetErrorMessage() => [ResourceMessagesException.EXPIRED_REFRESH_TOKEN];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}
