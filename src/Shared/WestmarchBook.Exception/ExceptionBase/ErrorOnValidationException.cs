namespace WestmarchBook.Exception.ExceptionBase;

public class ErrorOnValidationException: WestmarchBookException
{
    private readonly List<string> _errors;

    public ErrorOnValidationException(List<string> errorsMessage)
    {
        _errors = errorsMessage;
    }

    public List<string> GetErrorMessage()
    {
        return _errors;
    }
}
