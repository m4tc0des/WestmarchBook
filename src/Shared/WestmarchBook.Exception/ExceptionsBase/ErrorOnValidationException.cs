using System.Net;

namespace WestmarchBook.Exception.ExceptionsBase;

public class ErrorOnValidationException: WestmarchBookException
{
    private readonly List<string> _errors;

    public ErrorOnValidationException(List<string> errorsMessage)
    {
        _errors = errorsMessage;
    }

    public override List<string> GetErrorMessage()
    {
        return _errors;
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.BadRequest;
    }
}
