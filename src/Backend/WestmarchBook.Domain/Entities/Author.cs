namespace WestmarchBook.Domain.Entities;

public class Author : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public bool HasImage { get; set; }
    public ICollection<BookAuthor> BookAuthors { get; set; } = [];
}
