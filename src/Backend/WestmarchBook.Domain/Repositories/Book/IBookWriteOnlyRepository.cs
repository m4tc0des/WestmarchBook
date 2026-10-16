namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookWriteOnlyRepository
{
    Task Add(Entities.Book book);
}
