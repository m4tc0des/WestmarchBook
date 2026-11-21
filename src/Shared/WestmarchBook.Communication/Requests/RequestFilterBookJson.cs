using WestmarchBook.Communication.Enums;

namespace WestmarchBook.Communication.Requests;

public class RequestFilterBookJson
{
    public string? SearchTerm { get; set; } = string.Empty;
    public Genre? Genre { get; set; }
    public long? PublisherId { get; set; }
    public long? AuthorId { get; set; }
}
