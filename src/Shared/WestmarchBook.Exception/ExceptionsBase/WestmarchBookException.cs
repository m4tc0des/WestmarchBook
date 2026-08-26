using System.Net;

namespace WestmarchBook.Exception.ExceptionsBase;

public abstract class WestmarchBookException: System.Exception
{
    public abstract HttpStatusCode GetStatusCode();
    public abstract List<string> GetErrorMessage();
}
