namespace WestmarchBook.Domain.Entities;

public class Genre : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<BookGenre> BookGenres { get; set; } = [];
}
