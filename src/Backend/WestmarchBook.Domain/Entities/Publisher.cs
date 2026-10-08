namespace WestmarchBook.Domain.Entities;

public class Publisher : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Book> Books { get; set; } = [];
}
