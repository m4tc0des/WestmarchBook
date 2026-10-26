namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookReadOnlyRepository
{
    Task<bool> ExistActiveBookWithTitle(string title);
    Task<Entities.Book?> GetById(long bookId, long userId);
}
