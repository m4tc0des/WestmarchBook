namespace WestmarchBook.Communication.Responses;

public class ResponseErrorJson
{
    public List<string> Errors { get; private set; }
    public bool AccessTokenExpired { get; private set; }
    public ResponseErrorJson(List<string> errorMessages)
    {
        Errors = errorMessages;        
    }

    public ResponseErrorJson(string errorMessage)
    {
        Errors = new List<string> { errorMessage };
    }

    public ResponseErrorJson(string errorMessage, bool accessTokenExpired)
    {
        Errors = new List<string> { errorMessage };
        AccessTokenExpired = accessTokenExpired;
    }
}
