namespace WestmarchBook.Domain.Entities;

public class Author : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<BookAuthor> BookAuthors { get; set; } = [];
}
