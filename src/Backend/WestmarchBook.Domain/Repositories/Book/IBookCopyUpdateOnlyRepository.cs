namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookCopyUpdateOnlyRepository
{
    Task<Entities.BookCopy?> GetById(long id);
    void Update(Entities.BookCopy bookCopy);
}
