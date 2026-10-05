namespace WestmarchBook.Communication.Responses;

public class ResponseRegisterUserJson
{
    public string UserName { get; set; } = string.Empty;
    public ResponseTokensJson Tokens { get; set; } = new ResponseTokensJson();
}
