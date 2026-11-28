namespace WestmarchBook.Domain.Entities;

public class BookGenre
{
    public long BookId { get; set; }
    public Book Book { get; set; } = null!;

    public long GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}
