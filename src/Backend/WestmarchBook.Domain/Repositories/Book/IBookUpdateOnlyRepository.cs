namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookUpdateOnlyRepository
{
    Task<Entities.Book?> GetById(long bookId);
    void Update(Entities.Book book);
}
