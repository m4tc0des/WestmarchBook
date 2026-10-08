namespace WestmarchBook.Domain.Entities;

public class BookAuthor
{
    public long BookId { get; set; }
    public Book Book { get; set; } = null!;

    public long AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}
