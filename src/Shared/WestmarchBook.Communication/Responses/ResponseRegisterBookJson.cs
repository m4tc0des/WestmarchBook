namespace WestmarchBook.Communication.Responses;

public class ResponseRegisterBookJson
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
