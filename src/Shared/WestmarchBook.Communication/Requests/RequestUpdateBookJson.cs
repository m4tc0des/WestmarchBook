using WestmarchBook.Communication.Enums;

namespace WestmarchBook.Communication.Requests;

public class RequestUpdateBookJson
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? PublicationDate { get; set; }
    public long? PublisherId { get; set; }
    public IList<string>? RegistrationNumbers { get; set; }
    public IList<long>? Authors { get; set; }
    public IList<Genre>? Genres { get; set; }
}
