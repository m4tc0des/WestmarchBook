using WestmarchBook.Communication.Enums;

namespace WestmarchBook.Communication.Requests;

public class RequestRegisterBookJson
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublicationDate { get; set; }
    public long PublisherId { get; set; }
    public IList<long> Authors { get; set; } = [];
    public IList<Genre> Genres { get; set; } = [];
    public IList<Language> Languages { get; set; } = [];
}
