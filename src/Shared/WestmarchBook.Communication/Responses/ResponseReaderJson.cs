namespace WestmarchBook.Communication.Responses;

public class ResponseReaderJson
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
}
