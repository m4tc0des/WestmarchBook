namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookDeleteOnlyRepository
{
    Task<bool> DeleteById(long bookId);
    Task<bool> InactiveById(long bookId);
}
