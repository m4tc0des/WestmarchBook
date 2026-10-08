using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.Entities;

public class Book : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<BookAuthor> BookAuthors { get; set; } = [];
    public long PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public DateTime PublicationDate { get; set; }
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<Language> Languages { get; set; } = [];
    public long UserId { get; set; }
}
