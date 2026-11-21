namespace WestmarchBook.Communication.Requests;

public class RequestRegisterReaderJson
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
