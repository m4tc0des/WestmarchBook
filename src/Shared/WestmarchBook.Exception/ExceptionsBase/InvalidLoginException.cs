using System.Net;

namespace WestmarchBook.Exception.ExceptionsBase;

public class InvalidLoginException : WestmarchBookException
{
    public override List<string> GetErrorMessage()
    {
        return new List<string> { ResourceMessagesException.VALIDATION_LOGIN_INVALID };
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.Unauthorized;
    }
}
