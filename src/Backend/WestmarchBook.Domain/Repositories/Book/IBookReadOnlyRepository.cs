using WestmarchBook.Domain.Dtos;

namespace WestmarchBook.Domain.Repositories.Book;

public interface IBookReadOnlyRepository
{
    Task<bool> ExistActiveBookWithTitle(string title);
    Task<bool> ExistActiveBookWithTitle(string title, long bookId);
    Task<Entities.Book?> GetById(long bookId);
    Task<long> GetLastBookId();
    Task<IList<Entities.Book>> GetAllActive();
    Task<IList<BookSummaryDto>> FilterBooks(BookFilterDto filter);
}
