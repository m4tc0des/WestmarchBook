using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories.Book;

namespace WestmarchBook.Infrastructure.DataAccess.Repositories;

internal sealed class BookRepository: IBookWriteOnlyRepository
{
    private readonly WestmarchBookDbContext _dbContext;

    public BookRepository(WestmarchBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Book book)
    {
        await _dbContext.Books.AddAsync(book);
    }
}
