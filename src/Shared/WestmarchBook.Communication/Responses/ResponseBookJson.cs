namespace WestmarchBook.Communication.Responses;

public class ResponseBookJson
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ResponsePublisherJson Publisher { get; set; } = null!;
    public List<ResponseAuthorJson> Authors { get; set; } = new List<ResponseAuthorJson>();
    public IList<string> Genres { get; set; } = [];
    public IList<string> Languages { get; set; } = [];
    public DateTime PublicationDate { get; set; }
}
