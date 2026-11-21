using System.Net;

namespace WestmarchBook.Exception.ExceptionsBase;

public class NotFoundException : WestmarchBookException
{
    private readonly string _message;
    public NotFoundException(string message)
    {
        _message = message;
    }

    public override List<string> GetErrorMessage()
    {
        return new List<string> { _message };
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.NotFound;
    }
}
