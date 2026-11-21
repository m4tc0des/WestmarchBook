namespace WestmarchBook.Communication.Requests;

public class RequestUpdateReaderJson
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string? City { get; set; }
}
